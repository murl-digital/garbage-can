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
using Microsoft.EntityFrameworkCore;
using Serilog;
using Z.EntityFramework.Plus;

namespace GarbageCan.Commands.Roles
{
    [Group("levelRoles"), Aliases("levelRole", "lr")]
    public class LevelRoleCommandModule : BaseCommandModule
    {
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public Task AddLevelRole(CommandContext ctx, int lvl, DiscordRole role, bool remain)
        {
            try
            {
                using var context = new Context();
                context.xpLevelRoles.Add(new EntityLevelRole
                {
                    lvl = lvl,
                    roleId = role.Id,
                    remain = remain
                });
                context.SaveChanges();
                ctx.RespondAsync("Role added successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't add level role");
                ctx.RespondAsync("An error occured");
            }
            
            return Task.CompletedTask;
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveLevelRole(CommandContext ctx, int id)
        {
            try
            {
                using var context = new Context();
                await context.xpLevelRoles.Where(r => r.id == id).DeleteAsync();
                await ctx.RespondAsync("Role removed successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't remove level role");
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
                await context.xpLevelRoles
                    .ForEachAsync(r =>
                    {
                        var role = ctx.Guild.GetRole(r.roleId);
                        builder.AppendLine($"{r.id} :: level {r.lvl} | {role.Name}");
                    });
                await ctx.RespondAsync(Formatter.BlockCode(builder.ToString()));
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't list level roles");
                await ctx.RespondAsync("An error occured");
            }
        }
    }
}