using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.Commands.PrintLevelRoles;
using System.Threading.Tasks;

namespace GarbageCan.Web.Commands.Roles
{
    [Group("levelRoles")]
    [Aliases("levelRole", "lr")]
    public class LevelRoleCommandModule : MediatorCommandModule
    {
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public Task AddLevelRole(CommandContext ctx, int lvl, DiscordRole role, bool remain)
        {
            return Task.CompletedTask;
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            await Mediator.Send(new PrintLevelRolesCommand(), ctx);
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public Task RemoveLevelRole(CommandContext ctx, int id)
        {
            return Task.CompletedTask;
        }
    }
}