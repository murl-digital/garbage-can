using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Moderation;

namespace GarbageCan.Commands.Moderation
{
    public class RestrictChannelCommandModule
    {
        [Command("restrict")]
        [RequireRoles(RoleCheckMode.All, "Staff")]
        public Task Restrict(CommandContext ctx, DiscordUser user, DiscordChannel channel, TimeSpan span,
            string comments)
        {
            ModManager.RestrictChannel(user.Id, channel.Id, span, comments);

            return Task.CompletedTask;
        }

        [Command("restrict")]
        public Task RestrictDefault(CommandContext ctx, DiscordUser user, DiscordChannel channel, string comments)
        {
            ModManager.RestrictChannel(user.Id, channel.Id, TimeSpan.FromHours(24), comments);
            
            return Task.CompletedTask;
        }
    }
}