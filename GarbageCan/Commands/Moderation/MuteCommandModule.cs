using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Moderation;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Commands.Moderation
{
    public class MuteCommandModule : BaseCommandModule
    {
        [Command("mute")]
        [RequireRoles(RoleCheckMode.All, "Staff")]
        public Task Mute(CommandContext ctx, DiscordUser user, TimeSpan span, string comments)
        {
            ModManager.Mute(user.Id, span, comments);
            
            return Task.CompletedTask;
        }

        [Command("mute")]
        public Task MuteDefault(CommandContext ctx, DiscordUser user, string comments)
        {
            ModManager.Mute(user.Id, TimeSpan.FromHours(1), comments);

            return Task.CompletedTask;
        }
    }
}