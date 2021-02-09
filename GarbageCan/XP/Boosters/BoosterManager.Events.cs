using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using GarbageCan.Data;
using GarbageCan.Data.Models.Boosters;
using Serilog;

namespace GarbageCan.XP.Boosters
{
    public partial class BoosterManager
    {
        private static void Ready()
        {
            try
            {
                using var context = new Context();

                var queuedEntities = context.xpQueuedBoosters
                    .Select(x => x)
                    .ToList();
                queuedEntities.Sort((x, y) => x.position.CompareTo(y.position));

                _queuedBoosters = new Queue<QueuedBooster>(queuedEntities.Select(x => new QueuedBooster
                    {durationInSeconds = x.durationInSeconds, multiplier = x.multiplier}).ToList());

                var now = DateTime.Now.ToUniversalTime();
                if (context.xpActiveBoosters.Any(x => x.expirationDate < now))
                {
                    foreach (var expired in context.xpActiveBoosters.Where(x => x.expirationDate < now).ToList())
                        context.xpActiveBoosters.Remove(expired);

                    context.SaveChanges();
                }

                foreach (var entity in context.xpActiveBoosters.Where(x => x.expirationDate < now).ToList())
                    ActiveBoosters.Add(new ActiveBooster
                    {
                        expirationDate = entity.expirationDate,
                        multiplier = entity.multipler,
                        slot = _availableSlots.Select(x => x).First(x => x.id == entity.slot.id)
                    });
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
    }
}