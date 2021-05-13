using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.Commands.PrintReactionRoles;
using System.Threading.Tasks;

namespace GarbageCan.Web.Commands.Roles
{
    [Group("reactionRoles")]
    [Aliases("reactionRole", "rr")]
    public class ReactionRoleCommandModule : MediatorCommandModule
    {
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task AddReactionRole(CommandContext ctx, DiscordMessage msg, DiscordEmoji emote, DiscordRole role)
        {
            await Task.Delay(300);
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            await Mediator.Send(new PrintReactionRolesCommand(), ctx);
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveReactionRole(CommandContext ctx, int id)
        {
            await Task.Delay(300);
        }
    }
}