﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Roles.Commands.AddReactionRole;
using GarbageCan.Application.Roles.Commands.PrintReactionRoles;
using GarbageCan.Application.Roles.Commands.RemoveReactionRole;
using GarbageCan.Domain.Entities;
using System.Threading.Tasks;

namespace GarbageCan.Web.Commands.Roles
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
                ChannelId = msg.ChannelId,
                MessageId = msg.Id,
                RoleId = role.Id,
                Emoji = new Emoji
                {
                    Id = emote.Id,
                    Name = emote.Name
                }
            }, ctx);
        }

        [Command("list")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task List(CommandContext ctx)
        {
            await Mediator.Send(new PrintReactionRolesCommand(), ctx);
        }

        [Command("remove")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task RemoveReactionRole(CommandContext ctx, int id)
        {
            await Mediator.Send(new RemoveReactionRoleCommand { Id = id }, ctx);
        }
    }
}