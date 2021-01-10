using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Data.Models.Moderation;
using GarbageCan.Moderation;

namespace GarbageCan.Commands.Moderation
{
    public class LogTalkCommandModule : BaseCommandModule
    {
        [Command("logPersonalTalk")]
        [Aliases("logTalk", "lt")]
        [RequirePermissions(Permissions.Administrator)]
        public Task LogTalk(CommandContext ctx, DiscordMember member, [RemainingText] string comments)
        {
            ActionLog entry;
            ModManager.Log(member.Id, ctx.Member.Id, PunishmentLevel.PersonalTalk, comments, out entry);
            ctx.RespondAsync(
                $"{GarbageCan.Check} 1 on 1 talk with {member.DisplayName} has been logged with id {entry.id}");
            
            return Task.CompletedTask;
        }
    }
}