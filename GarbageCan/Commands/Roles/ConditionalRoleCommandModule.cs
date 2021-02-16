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
using Serilog;
using Z.EntityFramework.Plus;

namespace GarbageCan.Commands.Roles
{
    [Group("conditionalRoles")]
    [Aliases("conditionalRole", "cr")]
    public class ConditionalRoleCommandModule : BaseCommandModule
    {
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task AddConditionalRole(CommandContext ctx, DiscordRole required, DiscordRole result, bool remain)
        {
            try
            {
                await using var context = new Context();
                context.conditionalRoles.Add(new EntityConditionalRole
                {
                    requiredRoleId = required.Id,
                    resultRoleId = result.Id,
                    remain = remain
                });
                await context.SaveChangesAsync();
                await ctx.RespondAsync("Role added successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't add conditional role");
                await ctx.RespondAsync("An error occured");
            }
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveConditionalRole(CommandContext ctx, int id)
        {
            try
            {
                using var context = new Context();
                await context.conditionalRoles.Where(r => r.id == id).DeleteAsync();
                await ctx.RespondAsync("Role removed successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't remove conditional role");
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
                foreach (var role in context.conditionalRoles)
                {
                    var resultRole = ctx.Guild.GetRole(role.resultRoleId);
                    var requiredRole = ctx.Guild.GetRole(role.requiredRoleId);
                    builder.AppendLine($"{role.id} :: {requiredRole.Name} | {resultRole.Name}");
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