using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Data;
using GarbageCan.Data.Entities.Boosters;
using GarbageCan.Data.Models.Boosters;
using Serilog;
using Z.EntityFramework.Plus;

namespace GarbageCan.XP.Boosters
{
    public partial class BoosterManager : IFeature
    {
        public static ReadOnlyCollection<ActiveBooster> activeBoosters => ActiveBoosters.AsReadOnly();
        public static ReadOnlyCollection<QueuedBooster> queuedBoosters => _queuedBoosters.ToList().AsReadOnly();
        public static ReadOnlyCollection<AvailableSlot> availableSlots => _availableSlots.AsReadOnly();

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

            client.Ready += (_, _) => { 
                Task.Run(async () => {
                    await Task.Run(Ready);
                
                    BoosterTimer.Elapsed += Tick;

                    BoosterTimer.Enabled = true;
                });

                return Task.CompletedTask;
            };

            client.GuildDownloadCompleted += (_, args) =>
            {
                _nitroBoosterCount = args.Guilds[GarbageCan.operatingGuildId].PremiumSubscriptionCount ?? _nitroBoosterCount;
                
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

        public static void AddSlot(DiscordChannel channel)
        {
            Task.Run(async () =>
            {
                await using var context = new Context();
                var slot = new EntityAvailableSlot
                {
                    channelId = channel.Id
                };
                context.xpAvailableSlots.Add(slot);
                await context.SaveChangesAsync();
                
                _availableSlots.Add(new AvailableSlot
                {
                    id = slot.id,
                    channelId = slot.channelId
                });
            });
        }

        public static void RemoveSlot(int id)
        {
            _availableSlots.Remove(_availableSlots.FirstOrDefault(s => s.id == id));

            Task.Run(async () =>
            {
                await using var context = new Context();
                await context.xpAvailableSlots.Where(s => s.id == id).DeleteAsync();
            });
        }
    }
}