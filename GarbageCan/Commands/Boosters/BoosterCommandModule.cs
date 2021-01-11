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
    [Group("boosters"), Aliases("booster")]
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
            catch (Exception)
            {
                await ctx.RespondAsync("An error occured");
            }
        }

        [Command("use")]
        public async Task UseBooster(CommandContext ctx, int id)
        {
            try
            {
                using var context = new Context();
                var booster = await context.xpUserBoosters.FirstOrDefaultAsync(b => b.id == id);
                if (booster == null || booster.userId != ctx.User.Id)
                {
                    await ctx.RespondAsync("No booster exists with that id");
                    return;
                }

                var result = BoosterManager.AddBooster(booster.multiplier, TimeSpan.FromSeconds(booster.durationInSeconds), true);
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

                context.xpUserBoosters.Remove(booster);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
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
            catch (Exception)
            {
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
                        await ctx.RespondAsync("Booster has been activated");
                        break;
                    case BoosterResult.Queued:
                        await ctx.RespondAsync("Booster has been queued");
                        break;
                    case BoosterResult.SlotsFull:
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
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
                await ctx.RespondAsync("Booster has been given");
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
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
                Log.Error(e.ToString());
                throw;
            }
        }
    }
}