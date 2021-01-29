using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
using GarbageCan.Data;
using GarbageCan.Data.Entities.Boosters;
using GarbageCan.Data.Models.Boosters;
using Serilog;
using Z.EntityFramework.Plus;

namespace GarbageCan.XP.Boosters
{
    public class BoosterManager : IFeature
    {
        private static List<AvailableSlot> _availableSlots;
        private static readonly List<ActiveBooster> ActiveBoosters = new();
        private static Queue<QueuedBooster> _queuedBoosters;
        
        private static int _nitroBoosterCount;

        private static readonly Timer BoosterTimer = new(5000);

        public void Init(DiscordClient client)
        {
            using (var context = new Context())
            {
                _availableSlots = context.xpAvailableSlots
                    .Select(slot => new AvailableSlot {channelId = slot.channelId, id = slot.id})
                    .ToList();
            }

            client.Ready += (_, _) =>
            {
                Task.Run(Ready);

                BoosterTimer.Elapsed += Tick;

                BoosterTimer.Enabled = true;

                return Task.CompletedTask;
            };

            client.GuildDownloadCompleted += (_, args) =>
            {
                _nitroBoosterCount = args.Guilds.First().Value.PremiumSubscriptionCount ?? _nitroBoosterCount;
                
                return Task.CompletedTask;
            };

            client.GuildUpdated += (_, args) =>
            {
                if (args.GuildAfter.PremiumSubscriptionCount > _nitroBoosterCount)
                    AddBooster(2.0f, new TimeSpan(0, 0, 90, 0), true);

                _nitroBoosterCount = args.GuildAfter.PremiumSubscriptionCount ?? _nitroBoosterCount;

                return Task.CompletedTask;
            };
        }

        public void Cleanup()
        {
            using var context = new Context();
            BoosterTimer.Enabled = false;
            foreach (var booster in ActiveBoosters.Where(booster =>
                !context.xpActiveBoosters.Any(x => x.slot.id == booster.slot.id)))
                context.xpActiveBoosters.Add(new EntityActiveBooster
                {
                    expirationDate = booster.expirationDate,
                    multipler = booster.multiplier,
                    slot = context.xpAvailableSlots.Find(booster.slot.id)
                });

            context.SaveChanges();
            SaveQueue();
        }

        public static float GetMultiplier()
        {
            return 1 + ActiveBoosters.Sum(booster => booster.multiplier - 1);
        }

        public static BoosterResult AddBooster(float multiplier, TimeSpan duration, bool queue)
        {
            if (ActiveBoosters.Count >= _availableSlots.Count)
            {
                if (!queue) return BoosterResult.SlotsFull;

                _queuedBoosters.Enqueue(new QueuedBooster
                {
                    multiplier = multiplier,
                    durationInSeconds = (long) duration.TotalSeconds
                });
                SaveQueue();
                return BoosterResult.Queued;
            }

            var usedSlots = ActiveBoosters
                .Select(b => b.slot)
                .ToList();

            var slot = _availableSlots
                .First(s => !usedSlots.Contains(s));

            ActivateBooster(multiplier, duration, slot);
            return BoosterResult.Active;
        }

        private static void Ready()
        {
            try
            {
                using var context = new Context();

                #region retrieve queued boosters from db

                var queuedEntities = context.xpQueuedBoosters
                    .Select(x => x)
                    .ToList();
                queuedEntities.Sort((x, y) => x.position.CompareTo(y.position));

                _queuedBoosters = new Queue<QueuedBooster>(queuedEntities.Select(x => new QueuedBooster
                    {durationInSeconds = x.durationInSeconds, multiplier = x.multiplier}).ToList());

                #endregion

                #region remove any active boosters in db are expired

                var now = DateTime.Now.ToUniversalTime();
                if (context.xpActiveBoosters.Any(x => x.expirationDate < now))
                {
                    foreach (var expired in context.xpActiveBoosters.Where(x => x.expirationDate < now).ToList())
                        context.xpActiveBoosters.Remove(expired);

                    context.SaveChanges();
                }

                #endregion

                #region retrieve remaining active boosters

                foreach (var entity in context.xpActiveBoosters.Where(x => x.expirationDate < now).ToList())
                    ActiveBoosters.Add(new ActiveBooster
                    {
                        expirationDate = entity.expirationDate,
                        multiplier = entity.multipler,
                        slot = _availableSlots.Select(x => x).First(x => x.id == entity.slot.id)
                    });

                #endregion
            }
            catch (Exception e)
            {
                Log.Error(e, "Booster manager ready failed");
            }
        }

        private static async void Tick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                var toProcess = new List<ActiveBooster>();

                using (var context = new Context())
                {
                    foreach (var activeBooster in ActiveBoosters.Where(activeBooster =>
                        activeBooster.expirationDate < DateTime.Now.ToUniversalTime()))
                    {
                        var entity = context.xpActiveBoosters
                            .First(x => x.slot.id == activeBooster.slot.id);
                        context.xpActiveBoosters.Remove(entity);

                        toProcess.Add(activeBooster);
                    }

                    await context.SaveChangesAsync();
                }

                var saveQueue = false;

                toProcess.ForEach(b =>
                {
                    ActiveBoosters.Remove(b);

                    if (_queuedBoosters.Count > 0)
                    {
                        saveQueue = true;
                        ActivateQueuedBooster(b.slot);
                    }
                    else
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                var channel = await GarbageCan.Client.GetChannelAsync(b.slot.channelId);
                                await channel.ModifyAsync(model => model.Name = "-");
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Couldn't update channel name");
                            }
                        });
                    }
                });

                while (_queuedBoosters.Count > 0 && ActiveBoosters.Count < _availableSlots.Count)
                {
                    saveQueue = true;

                    var usedSlots = ActiveBoosters
                        .Select(b => b.slot)
                        .ToList();

                    var slot = _availableSlots
                        .First(s => !usedSlots.Contains(s));

                    ActivateQueuedBooster(slot);
                }

                if (saveQueue) SaveQueue();
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't execute BoosterManager cycle");
            }
        }

        private static void ActivateQueuedBooster(AvailableSlot slot)
        {
            var booster = _queuedBoosters.Dequeue();
            ActivateBooster(booster.multiplier, TimeSpan.FromSeconds(booster.durationInSeconds), slot);
        }

        private static void ActivateBooster(float multiplier, TimeSpan duration, AvailableSlot slot)
        {
            try
            {
                var booster = new ActiveBooster
                {
                    expirationDate = DateTime.Now.ToUniversalTime().Add(duration),
                    multiplier = multiplier,
                    slot = slot
                };

                ActiveBoosters.Add(booster);

                using (var context = new Context())
                {
                    context.xpActiveBoosters.Add(new EntityActiveBooster
                    {
                        expirationDate = booster.expirationDate,
                        multipler = booster.multiplier,
                        slot = context.xpAvailableSlots.Find(booster.slot.id)
                    });
                    context.SaveChanges();
                }

                Task.Run(async () =>
                {
                    try
                    {
                        var channel = await GarbageCan.Client.GetChannelAsync(booster.slot.channelId);
                        await channel.ModifyAsync(model => model.Name = GetBoosterString(booster));
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Couldn't update channel name");
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't activate booster");
            }
        }

        private static string GetBoosterString(Booster booster)
        {
            return $"{booster.multiplier.ToString(CultureInfo.CurrentCulture)}x";
        }

        private static void SaveQueue()
        {
            Task.Run(async () =>
            {
                try
                {
                    using var context = new Context();
                    context.xpQueuedBoosters.Delete();
                    var position = 0;
                    foreach (var booster in _queuedBoosters)
                    {
                        context.xpQueuedBoosters.Add(new EntityQueuedBooster
                        {
                            durationInSeconds = booster.durationInSeconds,
                            multiplier = booster.multiplier,
                            position = position
                        });
                        position++;
                    }

                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Log.Error(e, "Queue couldn't be saved");
                }
            });
        }
    }
}