using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Data.Models.Moderation;
using GarbageCan.Moderation;

namespace GarbageCan.Commands.Moderation
{
    public class LogWarningCommandModule : BaseCommandModule
    {
        [Command("logWarning")]
        [Aliases("lw")]
        [RequireRoles(RoleCheckMode.All, "Staff")]
        public Task WarnCommand(CommandContext ctx, DiscordUser user, [RemainingText] string comments)
        {
            ActionLog entry;
            ModManager.Log(user.Id, ctx.Member.Id, PunishmentLevel.VerbalWarning, comments, out entry);
            ctx.RespondAsync($"{GarbageCan.Check} Verbal warning logged with id {entry.id}");            
            
            return Task.CompletedTask;
        }
    }
}