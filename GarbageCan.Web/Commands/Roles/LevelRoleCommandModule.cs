using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.Commands.AddLevelRole;
using GarbageCan.Application.Roles.Commands.PrintLevelRoles;
using GarbageCan.Application.Roles.Commands.RemoveLevelRole;
using System.Threading.Tasks;

namespace GarbageCan.Web.Commands.Roles
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
                RoleId = role.Id,
                Level = lvl,
                Remain = remain
            }, ctx);
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            await Mediator.Send(new PrintLevelRolesCommand(), ctx);
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveLevelRole(CommandContext ctx, int id)
        {
            await Mediator.Send(new RemoveLevelRoleCommand { Id = id }, ctx);
        }
    }
}