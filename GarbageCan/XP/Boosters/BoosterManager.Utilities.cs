using System;
using System.Globalization;
using System.Threading.Tasks;
using GarbageCan.Data;
using GarbageCan.Data.Entities.Boosters;
using GarbageCan.Data.Models.Boosters;
using Serilog;
using Z.EntityFramework.Plus;

namespace GarbageCan.XP.Boosters
{
    public partial class BoosterManager
    {
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

                Task.Run(async () =>
                {
                    await using var context = new Context();
                    context.xpActiveBoosters.Add(new EntityActiveBooster
                    {
                        expirationDate = booster.expirationDate,
                        multipler = booster.multiplier,
                        slot = context.xpAvailableSlots.Find(booster.slot.id)
                    });
                    await context.SaveChangesAsync();
                });

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

        private static string GetBoosterString(IBooster booster)
        {
            return $"{(booster.multiplier + 1).ToString(CultureInfo.CurrentCulture)}x";
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