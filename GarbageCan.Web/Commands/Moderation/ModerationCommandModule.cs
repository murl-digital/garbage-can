using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Moderation.Commands.LogTalk;
using GarbageCan.Application.Moderation.Commands.LogWarning;
using System.Threading.Tasks;

namespace GarbageCan.Web.Commands.Moderation
{
    public class ModerationCommandModule : MediatorCommandModule
    {
        [Command("logPersonalTalk")]
        [Aliases("logTalk", "lt")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task LogTalk(CommandContext ctx, DiscordMember member, [RemainingText] string comments)
        {
            await Mediator.Send(new LogTalkCommand { Comments = comments, UserId = member.Id, DisplayName = member.DisplayName }, ctx);
        }

        [Command("logWarning")]
        [Aliases("lw")]
        [RequireRoles(RoleCheckMode.All, "Staff")]
        public async Task LogWarning(CommandContext ctx, DiscordMember member, [RemainingText] string comments)
        {
            await Mediator.Send(new LogWarningCommand { Comments = comments, UserId = member.Id }, ctx);
        }
    }
}