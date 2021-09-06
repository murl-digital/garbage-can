using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.ConditionalRoles.Commands.AddConditionalRole;
using GarbageCan.Application.Roles.ConditionalRoles.Commands.RemoveConditionalRole;
using GarbageCan.Application.Roles.ConditionalRoles.Queries.GetGuildConditionalRoles;

namespace GarbageCan.Web.Commands.Roles
{
    [Group("conditionalRoles")]
    [Aliases("conditionalRole", "cr")]
    public class ConditionalRoleCommandModule : MediatorCommandModule
    {
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task AddConditionalRole(CommandContext ctx, DiscordRole required, DiscordRole result, bool remain)
        {
            await Mediator.Send(new AddConditionalRoleCommand
            {
                GuildId = ctx.Guild.Id,
                RequiredRoleId = required.Id,
                ResultRoleId = result.Id,
                Remain = remain
            }, ctx);

            await Mediator.RespondAsync(ctx, "Role added successfully", true);
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            var conditionalRoles = await Mediator.Send(new GetGuildConditionalRolesQuery
            {
                GuildId = ctx.Guild.Id
            }, ctx);

            if (!conditionalRoles.Any())
            {
                await Mediator.RespondAsync(ctx, "No conditional roles found!", formatAsBlock: true);
                return;
            }

            var roleIds = conditionalRoles.Select(x => x.ResultRoleId)
                .Concat(conditionalRoles.Select(x => x.RequiredRoleId));

            var lines = conditionalRoles
                .Select(x =>
                    $"{x.Id} :: {GetRoleName(ctx.Guild, x.RequiredRoleId)} | {GetRoleName(ctx.Guild, x.ResultRoleId)}")
                .ToList();

            await Mediator.RespondAsync(ctx, string.Join(Environment.NewLine, lines), formatAsBlock: true);
        }


        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveConditionalRole(CommandContext ctx, int id)
        {
            await Mediator.Send(new RemoveConditionalRoleCommand { Id = id }, ctx);
            await Mediator.RespondAsync(ctx, "Role removed successfully", true);
        }
    }
}
