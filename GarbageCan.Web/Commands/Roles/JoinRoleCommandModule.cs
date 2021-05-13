using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.Commands.PrintJoinRoles;

namespace GarbageCan.Web.Commands.Roles
{
    [Group("joinRoles")]
    [Aliases("joinRole", "jr")]
    public class JoinRoleCommandModule : MediatorCommandModule
    {
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public Task AddJoinRole(CommandContext ctx, DiscordRole role)
        {
            return Task.CompletedTask;
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            await Mediator.Send(new PrintJoinRolesCommand(), ctx);
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public Task RemoveJoinRole(CommandContext ctx, int id)
        {
            return Task.CompletedTask;
        }
    }
}