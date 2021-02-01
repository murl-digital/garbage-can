using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Data;
using GarbageCan.Data.Entities.Roles;
using GarbageCan.Roles;
using Serilog;
using Z.EntityFramework.Plus;

namespace GarbageCan.Commands.Roles
{
    [Group("joinRoles"), Aliases("joinRole", "jr")]
    public class JoinRoleCommandModule : BaseCommandModule
    {
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task AddJoinRole(CommandContext ctx, DiscordRole role)
        {
            try
            {
                using var context = new Context();
                context.joinRoles.Add(new EntityJoinRole
                {
                    roleId = role.Id
                });
                await context.SaveChangesAsync();
                await ctx.RespondAsync("Role added successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't add join role");
                await ctx.RespondAsync("An error occured");
            }
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveJoinRole(CommandContext ctx, int id)
        {
            try
            {
                using var context = new Context();
                await context.joinRoles.Where(r => r.id == id).DeleteAsync();
                await ctx.RespondAsync("Role removed successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't remove join role");
                await ctx.RespondAsync("An error occured");
            }
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            var builder = new StringBuilder();
            try
            {
                using var context = new Context();
                foreach (var r in context.joinRoles)
                {
                    var role = ctx.Guild.GetRole(r.roleId);
                    builder.AppendLine($"{r.id} :: {role.Name}");
                }
                await ctx.RespondAsync(Formatter.BlockCode(builder.ToString()));
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't list reaction roles");
                await ctx.RespondAsync("An error occured");
            }
        }
    }
}