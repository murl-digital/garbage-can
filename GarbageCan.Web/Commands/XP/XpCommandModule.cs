using System;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using GarbageCan.Application.XP.Queries.GetTopUsersByXP;
using GarbageCan.Application.XP.Queries.GetUserLevelImage;

namespace GarbageCan.Web.Commands.XP
{
    public class XpCommandModule : MediatorCommandModule
    {
        [Command("level")]
        [Aliases("lvl", "rank")]
        public async Task LevelCommand(CommandContext ctx)
        {
            await PrintUserLevelCommand(ctx, ctx.Member);
        }

        [Command("level")]
        public async Task LevelCommand(CommandContext ctx, DiscordMember member)
        {
            await PrintUserLevelCommand(ctx, member);
        }


        [Command("top")]
        public async Task TopCommand(CommandContext ctx)
        {
            var result = await Mediator.Send(new GetTopUsersByXPQuery
            {
                CurrentUserId = ctx.User.Id,
                Count = 10
            }, ctx);

            var topTenUsers = result.TopTenUsers;
            topTenUsers.Add(result.ContextUser);

            var usersWithDisplayNames = new List<UserTemp>();
            foreach (var user in topTenUsers)
            {
                var displayName = await GetMemberDisplayName(ctx, user);
                usersWithDisplayNames.Add(new UserTemp
                {
                    Place = user.Place,
                    Lvl = user.User.Lvl,
                    XP = user.User.XP,
                    DisplayName = displayName
                });
            }

            var lines = usersWithDisplayNames
                .Select(u => $"{u.Place} - {u.DisplayName} (lvl {u.Lvl}, {u.XP} xp)")
                .ToList();
            lines.Insert(lines.Count - 1, "--");

            await Mediator.RespondAsync(ctx, string.Join(Environment.NewLine, lines), formatAsBlock: true);
        }


        private async Task PrintUserLevelCommand(CommandContext ctx, DiscordMember member)
        {
            await ctx.Channel.TriggerTypingAsync();

            var steam = await Mediator.Send(new GetUserLevelImageQuery
            {
                UserId = member.Id,
                AvatarUrl = member.AvatarUrl,
                MessageId = ctx.Message.Id,
                MemberDiscriminator = member.Discriminator,
                UserDisplayName = member.DisplayName
            }, ctx);

            await Mediator.RespondAsync(ctx, "rank.png", steam, ctx.Message.Id, true);
        }

        private static async Task<string> GetMemberDisplayName(CommandContext ctx, GetTopUsersByXPQueryVmUser user)
        {
            try
            {
                return (await ctx.Guild.GetMemberAsync(user.User.UserId))?.DisplayName;
            }
            catch
            {
                return null;
            }
        }

        private class UserTemp
        {
            public int Place { get; init; }
            public int Lvl { get; init; }
            public double XP { get; init; }
            public string DisplayName { get; init; }
        }
    }
}
