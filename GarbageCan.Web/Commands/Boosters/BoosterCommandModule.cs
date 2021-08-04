using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Boosters.ActiveBoosters.Queries;
using GarbageCan.Application.Boosters.AvailableSlots.Queries;
using GarbageCan.Application.Boosters.Commands;
using GarbageCan.Application.Boosters.Queries;
using GarbageCan.Application.Boosters.QueuedBoosters.Queries;
using GarbageCan.Application.Boosters.UserBoosters.Commands;
using GarbageCan.Domain.Enums;
using Humanizer;
using Serilog;

namespace GarbageCan.Web.Commands.Boosters
{
    [Group("boosters")]
    [Aliases("booster")]
    public class BoosterCommandModule : MediatorCommandModule
    {
        [GroupCommand]
        public async Task GetBoosters(CommandContext ctx)
        {
            try
            {
                var boosters = await GetBoostersString(ctx, ctx.Guild.Id, ctx.User.Id);
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
                var booster = await Mediator.Send(new GetUserBoosterQuery
                {
                    GuildId = ctx.Guild.Id,
                    UserId = ctx.User.Id,
                    Id = id
                }, ctx);
                if (booster == null)
                {
                    await ctx.RespondAsync("No booster exists with that id");
                    return;
                }

                var result = await Mediator.Send(new AddBoosterCommand
                {
                    GuildId = ctx.Guild.Id,
                    Multiplier = booster.Multiplier,
                    Duration = TimeSpan.FromSeconds(booster.DurationInSeconds),
                    Queue = true
                }, ctx);
                switch (result)
                {
                    case BoosterResult.Active:
                        await ctx.RespondAsync("Your booster has been activated!");
                        break;
                    case BoosterResult.Queued:
                        await ctx.RespondAsync("Your booster has been queued!");
                        break;
                    case BoosterResult.SlotsFull:
                        break;
                }

                await Mediator.Send(new RemoveUserBoosterCommand { Id = booster.Id }, ctx);
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
                foreach (var booster in await Mediator.Send(new GetActiveBoostersQuery { GuildId = ctx.Guild.Id }, ctx))
                    activeBuilder.AppendLine(
                        $"slot {booster.Slot.Id} :: {booster.Multiplier}x expiring {booster.ExpirationDate.Humanize()}");
                if (activeBuilder.Length == 0) activeBuilder.AppendLine("No boosters");

                var queuedBuilder = new StringBuilder();
                var queue = (await Mediator.Send(new GetQueuedBoostersQuery { GuildId = ctx.Guild.Id }, ctx)).ToList();
                foreach (var booster in queue)
                    queuedBuilder.AppendLine(
                        $"{queue.IndexOf(booster)} :: {booster.Multiplier}x for {TimeSpan.FromSeconds(booster.DurationInSeconds).Humanize()}");
                if (queuedBuilder.Length == 0) queuedBuilder.AppendLine("No boosters");

                builder.AppendLine("-- Active Boosters --");
                builder.AppendLine(activeBuilder.ToString());
                builder.AppendLine("-- Queued Boosters --");
                builder.AppendLine(queuedBuilder.ToString());
                builder.AppendLine(
                    $"Current active multiplier: {await Mediator.Send(new GetBoosterMultiplierCommand { GuildId = ctx.Guild.Id }, ctx)}");

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
                var boosters = await GetBoostersString(ctx, ctx.Guild.Id, user.Id);
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
                var result = await Mediator.Send(new AddBoosterCommand
                {
                    GuildId = ctx.Guild.Id,
                    Duration = span,
                    Multiplier = multiplier,
                    Queue = queue
                }, ctx);
                switch (result)
                {
                    case BoosterResult.Active:
                        await ctx.RespondAsync("Booster has been activated!");
                        break;
                    case BoosterResult.Queued:
                        await ctx.RespondAsync("Booster has been queued");
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
        public async Task GiveBooster(CommandContext ctx, DiscordMember user, float multiplier, TimeSpan span)
        {
            try
            {
                await Mediator.Send(new AddUserBoosterCommand
                {
                    GuildId = ctx.Guild.Id,
                    UserId = user.Id,
                    Multiplier = multiplier,
                    Duration = span
                }, ctx);
                await ctx.RespondAsync("Booster has been given");
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't give booster");
                await ctx.RespondAsync("An error occured");
            }
        }

        private async Task<string> GetBoostersString(CommandContext ctx, ulong guildId, ulong userId)
        {
            try
            {
                var builder = new StringBuilder();
                (await Mediator.Send(new GetUserBoostersQuery
                    {
                        GuildId = guildId,
                        UserId = userId
                    }, ctx)).Select(b =>
                        $"{b.Id} :: {b.Multiplier}x for {TimeSpan.FromSeconds(b.DurationInSeconds).Humanize()}")
                    .ToList()
                    .ForEach(s => builder.AppendLine(s));

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
        public class SlotCommandModule : MediatorCommandModule
        {
            [GroupCommand]
            [RequirePermissions(Permissions.Administrator)]
            public async Task GetSlots(CommandContext ctx)
            {
                try
                {
                    var builder = new StringBuilder();
                    foreach (var slot in await Mediator.Send(new GetAvailableSlotsQuery { GuildId = ctx.Guild.Id },
                        ctx))
                        builder.AppendLine(
                            $"{slot.Id} :: {slot.ChannelId}");

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
                    await Mediator.Send(new AddSlotCommand { GuildId = ctx.Guild.Id, ChannelId = channel.Id }, ctx);
                    await ctx.RespondAsync("Slot successfully added");
                }
                catch (Exception e)
                {
                    Log.Error(e, "Couldn't add available slot");
                    await ctx.RespondAsync("Adding slot failed. Check the logs for details");
                }
            }

            [Command("remove")]
            [RequirePermissions(Permissions.Administrator)]
            public async Task RemoveSlot(CommandContext ctx, int id)
            {
                try
                {
                    try
                    {
                        await Mediator.Send(new RemoveSlotCommand { guildId = ctx.Guild.Id, id = id }, ctx);
                    }
                    catch (ArgumentException)

                    {
                        await ctx.RespondAsync("No slot found");
                        return;
                    }

                    await ctx.RespondAsync("Slot has been removed");
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
