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
    [Group("reactionRoles")]
    [Aliases("reactionRole", "rr")]
    public class ReactionRoleCommandModule : BaseCommandModule
    {
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task AddReactionRole(CommandContext ctx, DiscordMessage msg, DiscordEmoji emote, DiscordRole role)
        {
            try
            {
                using var context = new Context();
                context.reactionRoles.Add(new EntityReactionRole
                {
                    channelId = msg.ChannelId,
                    messageId = msg.Id,
                    emoteId = RoleManager.EmoteId(emote),
                    roleId = role.Id
                });
                await context.SaveChangesAsync();
                await msg.CreateReactionAsync(emote);
                await ctx.RespondAsync($"{GarbageCan.Check} Role added successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't add reaction role");
                await ctx.RespondAsync("An error occured");
            }
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveReactionRole(CommandContext ctx, int id)
        {
            try
            {
                using var context = new Context();
                await context.reactionRoles.Where(r => r.id == id).DeleteAsync();
                await ctx.RespondAsync($"{GarbageCan.Check} Role removed successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Couldn't remove reaction role");
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
                foreach (var r in context.reactionRoles)
                {
                    var role = ctx.Guild.GetRole(r.roleId);
                    var msg = await ctx.Guild.GetChannel(r.channelId).GetMessageAsync(r.messageId);
                    builder.AppendLine($"{r.id} :: msg {msg.Id} in #{msg.Channel.Name} | {role.Name}");
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