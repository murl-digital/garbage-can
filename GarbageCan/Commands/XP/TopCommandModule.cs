using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Exceptions;
using GarbageCan.Data;
using GarbageCan.Data.Models.XP;
using Serilog;

namespace GarbageCan.Commands.XP
{
    public class TopCommandModule : BaseCommandModule
    {
        [Command("top")]
        public Task TopCommand(CommandContext ctx)
        {
            Task.Run(async () =>
            {
                try
                {
                    await using var context = new Context();
                    var users = context.xpUsers
                        .OrderByDescending(u => u.xp)
                        .Select(u => new User
                        {
                            id = u.id,
                            lvl = u.lvl,
                            xp = u.xp
                        })
                        .ToArray();
                    var count = users.Length > 10 ? 10 : users.Length;
                    var builder = new StringBuilder();
                    for (var i = 0; i < count; i++)
                    {
                        string displayName;
                        try
                        {
                            displayName = (await ctx.Guild.GetMemberAsync(users[i].id)).DisplayName;
                        }
                        catch (NotFoundException)
                        {
                            displayName = "[member left]";
                        }
                        builder.AppendFormat("{0} - {1} (lvl {2}, {3} xp)", i + 1, displayName, users[i].lvl,
                            users[i].xp);
                        builder.AppendLine();
                    }

                    var senderIndex = Array.FindIndex(users, u => u.id == ctx.Member.Id);

                    builder.AppendLine("--");
                    builder.AppendFormat("{0} - {1} (lvl {2}, {3} xp)", senderIndex + 1, ctx.Member.DisplayName,
                        users[senderIndex].lvl, users[senderIndex].xp);

                    await ctx.RespondAsync(Formatter.BlockCode(builder.ToString()));
                }
                catch (Exception e)
                {
                    await ctx.RespondAsync("oof top isnt available rn");
                    Log.Error(e, "Top command failed");
                }
            });
            
            return Task.CompletedTask;
        }
    }
}