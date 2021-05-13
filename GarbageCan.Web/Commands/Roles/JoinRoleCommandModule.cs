using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.Commands.AddJoinRole;
using GarbageCan.Application.Roles.Commands.PrintJoinRoles;
using System.Threading.Tasks;

namespace GarbageCan.Web.Commands.Roles
{
    [Group("joinRoles")]
    [Aliases("joinRole", "jr")]
    public class JoinRoleCommandModule : MediatorCommandModule
    {
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task AddJoinRole(CommandContext ctx, DiscordRole role)
        {
            await Mediator.Send(new AddJoinRoleCommand { RoleId = role.Id }, ctx);
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