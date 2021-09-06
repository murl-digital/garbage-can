using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.ReactionRoles.Commands.AddReactionRole;
using GarbageCan.Application.Roles.ReactionRoles.Commands.RemoveReactionRole;
using GarbageCan.Application.Roles.ReactionRoles.Queries.GetGuildReactionRoles;
using GarbageCan.Domain.Entities;

namespace GarbageCan.Commands.Roles
{
    [Group("reactionRoles")]
    [Aliases("reactionRole", "rr")]
    public class ReactionRoleCommandModule : MediatorCommandModule
    {
        [Command("add")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task AddReactionRole(CommandContext ctx, DiscordMessage msg, DiscordEmoji emote, DiscordRole role)
        {
            await Mediator.Send(new AddReactionRoleCommand
            {
                GuildId = msg.Channel.Guild.Id,
                ChannelId = msg.ChannelId,
                MessageId = msg.Id,
                RoleId = role.Id,
                Emoji = new Emoji
                {
                    Id = emote.Id,
                    Name = emote.Name
                }
            }, ctx);

            await Mediator.RespondAsync(ctx, "Role added successfully", true);
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            var reactionRoles = await Mediator.Send(new GetGuildReactionRolesQuery
            {
                GuildId = ctx.Guild.Id
            }, ctx);

            if (!reactionRoles.Any())
            {
                await Mediator.RespondAsync(ctx, "No reaction roles found!", formatAsBlock: true);
                return;
            }

            var lines = reactionRoles
                .Select(x =>
                    $"{x.Id} :: msg {x.MessageId} in #{GetChannelName(ctx.Guild, x.ChannelId)} | {GetRoleName(ctx.Guild, x.RoleId)}")
                .ToList();
            await Mediator.RespondAsync(ctx, string.Join(Environment.NewLine, lines), formatAsBlock: true);
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveReactionRole(CommandContext ctx, int id)
        {
            // TODO: its possible to remove EVERY reaction role, even roles that you shouldn't have access to. whoops!
            await Mediator.Send(new RemoveReactionRoleCommand { Id = id }, ctx);
            await Mediator.RespondAsync(ctx, "Role removed successfully", true);
        }
    }
}
