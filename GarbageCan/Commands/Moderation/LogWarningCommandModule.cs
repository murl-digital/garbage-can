using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Moderation;

namespace GarbageCan.Commands.Moderation
{
    public class LogWarningCommandModule : BaseCommandModule
    {
        [Command("logWarning")]
        [RequirePermissions(Permissions.ViewAuditLog)]
        public Task WarnCommand(CommandContext ctx, DiscordUser user, string comments)
        {
            ModManager.Log(user.Id, PunishmentLevel.VerbalWarning, comments);
            
            return Task.CompletedTask;
        }
    }
}