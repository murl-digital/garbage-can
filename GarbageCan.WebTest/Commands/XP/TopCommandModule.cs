using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using GarbageCan.Application.XP.Queries.GetTopUsersByXP;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCan.WebTest.Commands.XP
{
    public class TopCommandModule : MediatorCommandModule
    {
        [Command("top")]
        public async Task TopCommand(CommandContext ctx)
        {
            var result = await Mediator.Send(new GetTopUsersByXPQuery { Count = 10, CurrentUserId = ctx.User.Id }, ctx);
            if (result.Succeeded)
            {
                var topTenUsers = result.Value.TopTenUsers;
                topTenUsers.Add(result.Value.ContextUser);
                var lines = topTenUsers.Select(u => $"{u.Place} - {u.DisplayName} (lvl {u.User.Lvl}, {u.User.XP} xp)").ToList();
                lines.Insert(lines.Count - 1, "--");

                await ctx.RespondAsync(Formatter.BlockCode(string.Join(Environment.NewLine, lines)));
            }
            else
            {
                await ctx.RespondAsync("there was a problem");
            }
        }
    }
}