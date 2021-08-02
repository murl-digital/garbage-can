using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.Commands.AddConditionalRole;
using GarbageCan.Application.Roles.Commands.PrintConditionalRoles;
using GarbageCan.Application.Roles.Commands.RemoveConditionalRole;

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
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            await Mediator.Send(new PrintConditionalRolesCommand
            {
                GuildId = ctx.Guild.Id
            }, ctx);
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveConditionalRole(CommandContext ctx, int id)
        {
            await Mediator.Send(new RemoveConditionalRoleCommand {Id = id}, ctx);
        }
    }
}
