using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Moderation;

namespace GarbageCan.Commands.Moderation
{
    public class MuteCommandModule : BaseCommandModule
    {
        [Command("mute")]
        [RequireRoles(RoleCheckMode.All, "Staff")]
        public Task Mute(CommandContext ctx, DiscordMember user, TimeSpan span, string comments)
        {
            ModManager.Mute(user, ctx.Member, span, comments);
            
            return Task.CompletedTask;
        }

        [Command("mute")]
        public Task MuteDefault(CommandContext ctx, DiscordMember user, string comments)
        {
            ModManager.Mute(user, ctx.Member, TimeSpan.FromHours(1), comments);

            return Task.CompletedTask;
        }
    }
}