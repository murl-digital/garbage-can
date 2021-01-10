using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Commands.Attributes;
using GarbageCan.Moderation;

namespace GarbageCan.Commands.Moderation
{
    public class BanCommandModule : BaseCommandModule
    {
        [Command("ban")]
        [RequireGuildOwner]
        public Task Ban(CommandContext ctx, DiscordMember member, [RemainingText] string reason)
        {
            member.BanAsync(reason: reason);
            ModManager.Log(member.Id, ctx.Member.Id, PunishmentLevel.Ban, reason);
            ctx.RespondAsync($"{GarbageCan.Check} {member.DisplayName} has been banned");
            
            return Task.CompletedTask;
        }
    }
}