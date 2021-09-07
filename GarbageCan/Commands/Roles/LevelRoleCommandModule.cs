using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.LevelRoles.Commands.AddLevelRole;
using GarbageCan.Application.Roles.LevelRoles.Commands.RemoveLevelRole;
using GarbageCan.Application.Roles.LevelRoles.Queries.GetGuildLevelRoles;

namespace GarbageCan.Commands.Roles
{
    [Group("levelRoles")]
    [Aliases("levelRole", "lr")]
    public class LevelRoleCommandModule : MediatorCommandModule
    {
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task AddLevelRole(CommandContext ctx, int lvl, DiscordRole role, bool remain)
        {
            await Mediator.Send(new AddLevelRoleCommand
            {
                GuildId = ctx.Guild.Id,
                RoleId = role.Id,
                Level = lvl,
                Remain = remain
            }, ctx);

            await Mediator.RespondAsync(ctx, "Role added successfully", true);
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            var levelRoles = await Mediator.Send(new GetGuildLevelRolesQuery
            {
                GuildId = ctx.Guild.Id
            }, ctx);

            if (!levelRoles.Any())
            {
                await Mediator.RespondAsync(ctx, "No level roles found!", formatAsBlock: true);
                return;
            }

            var lines = levelRoles
                .Select(x => $"{x.Id} :: level {x.Lvl} | {GetRoleName(ctx.Guild, x.RoleId)}")
                .ToList();
            await Mediator.RespondAsync(ctx, string.Join(Environment.NewLine, lines), formatAsBlock: true);
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveLevelRole(CommandContext ctx, int id)
        {
            // TODO: same issue as reaction roles here
            await Mediator.Send(new RemoveLevelRoleCommand { Id = id }, ctx);

            await Mediator.RespondAsync(ctx, "Role removed successfully", true);
        }
    }
}
