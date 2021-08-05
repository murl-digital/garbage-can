using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.JoinRoles.Commands.AddJoinRole;
using GarbageCan.Application.Roles.JoinRoles.Commands.PrintJoinRoles;
using GarbageCan.Application.Roles.JoinRoles.Commands.RemoveJoinRole;

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
            await Mediator.Send(new AddJoinRoleCommand
            {
                GuildId = ctx.Guild.Id,
                RoleId = role.Id
            }, ctx);
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            await Mediator.Send(new PrintJoinRolesCommand
            {
                GuildId = ctx.Guild.Id
            }, ctx);
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveJoinRole(CommandContext ctx, int id)
        {
            await Mediator.Send(new RemoveJoinRoleCommand {Id = id}, ctx);
        }
    }
}
