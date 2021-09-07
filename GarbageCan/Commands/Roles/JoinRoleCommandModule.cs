using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.JoinRoles.Commands.AddJoinRole;
using GarbageCan.Application.Roles.JoinRoles.Commands.RemoveJoinRole;
using GarbageCan.Application.Roles.JoinRoles.Queries.GetGuildJoinRoles;

namespace GarbageCan.Commands.Roles
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

            await Mediator.RespondAsync(ctx, "Role added successfully", true);
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            var joinRoles = await Mediator.Send(new GetGuildJoinRolesQuery
            {
                GuildId = ctx.Guild.Id
            }, ctx);

            if (!joinRoles.Any())
            {
                await Mediator.RespondAsync(ctx, "No join roles found!", formatAsBlock: true);
                return;
            }

            var lines = joinRoles
                .Select(x => $"{x.Id} :: {GetRoleName(ctx.Guild, x.RoleId)}")
                .ToList();
            await Mediator.RespondAsync(ctx, string.Join(Environment.NewLine, lines), formatAsBlock: true);
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveJoinRole(CommandContext ctx, int id)
        {
            await Mediator.Send(new RemoveJoinRoleCommand { Id = id }, ctx);
            await Mediator.RespondAsync(ctx, "Role removed successfully", true);
        }
    }
}
