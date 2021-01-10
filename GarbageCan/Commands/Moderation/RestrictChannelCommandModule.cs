using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Moderation;
using Humanizer;

namespace GarbageCan.Commands.Moderation
{
    public class RestrictChannelCommandModule
    {
        [Command("restrict")]
        [RequireRoles(RoleCheckMode.All, "Staff")]
        public Task Restrict(CommandContext ctx, DiscordMember member, DiscordChannel channel, TimeSpan span,
            [RemainingText] string comments)
        {
            ModManager.RestrictChannel(member, ctx.Member, channel, span, comments);
            ctx.RespondAsync(
                $"{GarbageCan.Check} {member.DisplayName}'s access to {channel} has been restricted for {span.Humanize()}");

            return Task.CompletedTask;
        }

        [Command("restrict")]
        public Task RestrictDefault(CommandContext ctx, DiscordMember member, DiscordChannel channel, 
            [RemainingText] string comments)
        {
            ModManager.RestrictChannel(member, ctx.Member, channel, TimeSpan.FromHours(24), comments);
            ctx.RespondAsync(
                $"{GarbageCan.Check} {member.DisplayName}'s access to {channel} has been restricted for 24 hours");

            return Task.CompletedTask;
        }
    }
}