using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Data;
using GarbageCan.Data.Entities.Boosters;
using GarbageCan.XP.Boosters;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GarbageCan.Commands.Boosters
{
    [Group("boosters")]
    [Aliases("booster")]
    public class BoosterCommandModule : BaseCommandModule
    {
        [GroupCommand]
        public async Task GetBoosters(CommandContext ctx)
        {
            try
            {
                var boosters = await GetBoostersString(ctx.User.Id);
                await ctx.RespondAsync(Formatter.BlockCode(boosters));
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't list boosters");
                await ctx.RespondAsync("An error occured");
            }
        }

        [Command("use")]
        public async Task UseBooster(CommandContext ctx, int id)
        {
            try
            {
                await using var context = new Context();
                var booster = await context.xpUserBoosters.FirstOrDefaultAsync(b => b.id == id);
                if (booster == null || booster.userId != ctx.User.Id)
                {
                    await ctx.RespondAsync("No booster exists with that id");
                    return;
                }

                var result = BoosterManager.AddBooster(booster.multiplier,
                    TimeSpan.FromSeconds(booster.durationInSeconds), true);
                switch (result)
                {
                    case BoosterResult.Active:
                        await ctx.RespondAsync($"{GarbageCan.Check} Your booster has been activated!");
                        break;
                    case BoosterResult.Queued:
                        await ctx.RespondAsync($"{GarbageCan.Check} Your booster has been queued!");
                        break;
                    case BoosterResult.SlotsFull:
                        break;
                }

                context.xpUserBoosters.Remove(booster);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't use booster");
                await ctx.RespondAsync("An error occured");
            }
        }

        [Command("list")]
        public async Task GetActiveBoosters(CommandContext ctx)
        {
            try
            {
                var builder = new StringBuilder();

                var activeBuilder = new StringBuilder();
                foreach (var booster in BoosterManager.activeBoosters)
                    activeBuilder.AppendLine(
                        $"slot {booster.slot.id} :: {booster.multiplier}x expiring {booster.expirationDate.Humanize()}");
                if (activeBuilder.Length == 0) activeBuilder.AppendLine("No boosters");

                var queuedBuilder = new StringBuilder();
                var queue = BoosterManager.queuedBoosters;
                foreach (var booster in queue)
                    queuedBuilder.AppendLine(
                        $"{queue.IndexOf(booster)} :: {booster.multiplier}x for {TimeSpan.FromSeconds(booster.durationInSeconds).Humanize()}");
                if (queuedBuilder.Length == 0) queuedBuilder.AppendLine("No boosters");

                builder.AppendLine("-- Active Boosters --");
                builder.AppendLine(activeBuilder.ToString());
                builder.AppendLine("-- Queued Boosters --");
                builder.AppendLine(queuedBuilder.ToString());
                builder.AppendLine($"Current active multiplier: {BoosterManager.GetMultiplier()}");

                await ctx.RespondAsync(Formatter.BlockCode(builder.ToString()));
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't get active boosters");
                await ctx.RespondAsync("An error occured");
            }
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task GetBoostersForUser(CommandContext ctx, DiscordUser user)
        {
            try
            {
                var boosters = await GetBoostersString(user.Id);
                await ctx.RespondAsync(Formatter.BlockCode(boosters));
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't get boosters");
                await ctx.RespondAsync("An error occured");
            }
        }

        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task AddBooster(CommandContext ctx, float multiplier, TimeSpan span, bool queue)
        {
            try
            {
                var result = BoosterManager.AddBooster(multiplier, span, queue);
                switch (result)
                {
                    case BoosterResult.Active:
                        await ctx.RespondAsync($"{GarbageCan.Check} Booster has been activated!");
                        break;
                    case BoosterResult.Queued:
                        await ctx.RespondAsync($"{GarbageCan.Check} Booster has been queued");
                        break;
                    case BoosterResult.SlotsFull:
                        break;
                    default:
                        throw new Exception($"AddBooster returned unexpected result: {result}");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't add booster");
                await ctx.RespondAsync("An error occured");
            }
        }

        [Command("give")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task GiveBooster(CommandContext ctx, DiscordUser user, float multiplier, TimeSpan span)
        {
            try
            {
                using var context = new Context();
                context.xpUserBoosters.Add(new EntityUserBooster
                {
                    userId = user.Id,
                    multiplier = multiplier,
                    durationInSeconds = (long) span.TotalSeconds
                });
                await context.SaveChangesAsync();
                await ctx.RespondAsync($"{GarbageCan.Check} Booster has been given");
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't give booster");
                await ctx.RespondAsync("An error occured");
            }
        }

        private async Task<string> GetBoostersString(ulong uId)
        {
            try
            {
                var builder = new StringBuilder();
                using var context = new Context();
                await context.xpUserBoosters
                    .Where(b => b.userId == uId)
                    .ForEachAsync(b =>
                        builder.AppendLine(
                            $"{b.id} :: {b.multiplier}x for {TimeSpan.FromSeconds(b.durationInSeconds).Humanize()}"));
                if (builder.Length == 0) builder.AppendLine("-- No boosters --");

                return builder.ToString();
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't generate boosters string");
                throw;
            }
        }

        [Group("slots")]
        [Aliases("slot")]
        public class SlotCommandModule : BaseCommandModule
        {
            [GroupCommand]
            [RequirePermissions(Permissions.Administrator)]
            public async Task GetSlots(CommandContext ctx)
            {
                try
                {
                    var builder = new StringBuilder();
                    foreach (var slot in BoosterManager.availableSlots)
                        builder.AppendLine(
                            $"{slot.id} :: {slot.channelId}");

                    if (builder.Length == 0) builder.AppendLine("No slots");

                    await ctx.RespondAsync(Formatter.BlockCode(builder.ToString()));
                }
                catch (Exception e)
                {
                    Log.Error(e, "Couldn't list slots");
                    await ctx.RespondAsync("Couldn't list available slots. Check logs for details.");
                }
            }

            [Command("add")]
            [RequirePermissions(Permissions.Administrator)]
            public async Task AddSlot(CommandContext ctx, DiscordChannel channel)
            {
                try
                {
                    BoosterManager.AddSlot(channel);
                    await ctx.RespondAsync($"{GarbageCan.Check} Slot successfully added");
                }
                catch (Exception e)
                {
                    Log.Error(e, "Couldn't add available slot");
                    await ctx.RespondAsync("Adding slot failed. Check the logs for details");
                }
            }

            public async Task RemoveSlot(CommandContext ctx, int id)
            {
                try
                {
                    await using var context = new Context();
                    if (context.xpAvailableSlots.Any(s => s.id == id))
                    {
                        await ctx.RespondAsync("No slot found");
                        return;
                    }

                    BoosterManager.RemoveSlot(id);
                    await ctx.RespondAsync($"{GarbageCan.Check} Slot has been removed");
                }
                catch (Exception e)
                {
                    Log.Error(e, "Couldn't remove slot");
                    await ctx.RespondAsync("An error occured, check logs for details.");
                }
            }
        }
    }
}