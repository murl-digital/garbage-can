using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.LevelRoles.Commands.AddLevelRole;
using GarbageCan.Application.Roles.LevelRoles.Commands.PrintLevelRoles;
using GarbageCan.Application.Roles.LevelRoles.Commands.RemoveLevelRole;

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
                GuildId = ctx.Guild.Id,
                RoleId = role.Id,
                Level = lvl,
                Remain = remain
            }, ctx);
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            await Mediator.Send(new PrintLevelRolesCommand
            {
                GuildId = ctx.Guild.Id
            }, ctx);
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveLevelRole(CommandContext ctx, int id)
        {
            // TODO: same issue as reaction roles here
            await Mediator.Send(new RemoveLevelRoleCommand {Id = id}, ctx);
        }
    }
}
