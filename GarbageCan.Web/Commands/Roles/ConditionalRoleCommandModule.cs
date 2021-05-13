using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.Commands.PrintConditionalRoles;

namespace GarbageCan.Web.Commands.Roles
{
    [Group("conditionalRoles")]
    [Aliases("conditionalRole", "cr")]
    public class ConditionalRoleCommandModule : MediatorCommandModule
    {
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public Task AddConditionalRole(CommandContext ctx, DiscordRole required, DiscordRole result, bool remain)
        {
            return Task.CompletedTask;
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            await Mediator.Send(new PrintConditionalRolesCommand(), ctx);
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public Task RemoveConditionalRole(CommandContext ctx, int id)
        {
            return Task.CompletedTask;
        }
    }
}