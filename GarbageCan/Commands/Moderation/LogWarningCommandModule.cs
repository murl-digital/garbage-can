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
        [RequireRoles(RoleCheckMode.All, "Staff")]
        public Task WarnCommand(CommandContext ctx, DiscordUser user, string comments)
        {
            ModManager.Log(user.Id, ctx.Member.Id, PunishmentLevel.VerbalWarning, comments);
            
            return Task.CompletedTask;
        }
    }
}