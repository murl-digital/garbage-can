using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Moderation;
using Serilog;

namespace GarbageCan.Commands.Moderation
{
    public class MuteCommandModule : BaseCommandModule
    {
        [Command("mute")]
        [RequireRoles(RoleCheckMode.All, "Staff")]
        public Task Mute(CommandContext ctx, DiscordMember member, TimeSpan span, [RemainingText] string comments)
        {
            ModManager.Mute(member, ctx.Member, span, comments);
            ctx.RespondAsync($"{GarbageCan.Check} {member.DisplayName} has been muted");
            
            return Task.CompletedTask;
        }

        [Command("mute")]
        public Task MuteDefault(CommandContext ctx, DiscordMember member, [RemainingText] string comments)
        {
            ModManager.Mute(member, ctx.Member, TimeSpan.FromHours(1), comments);
            ctx.RespondAsync($"{GarbageCan.Check} {member.DisplayName} has been muted");

            return Task.CompletedTask;
        }
    }
}