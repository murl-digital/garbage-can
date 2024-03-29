﻿using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.Moderation.Commands.Ban;
using GarbageCan.Application.Moderation.Commands.LogTalk;
using GarbageCan.Application.Moderation.Commands.LogWarning;
using GarbageCan.Application.Moderation.Commands.Mute;
using GarbageCan.Application.Moderation.Commands.Restrict;
using Humanizer;

namespace GarbageCan.Commands.Moderation
{
    public class ModerationCommandModule : MediatorCommandModule
    {
        [Command("ban")]
        public async Task Ban(CommandContext ctx, DiscordMember member, [RemainingText] string reason)
        {
            if (!ctx.Member.IsOwner)
            {
                return;
            }

            await Mediator.Send(new BanCommand
            {
                GuildId = ctx.Guild.Id,
                UserId = member.Id,
                Reason = reason,
            }, ctx);

            await Mediator.RespondAsync(ctx, $"{member.DisplayName} has been banned", true);
        }

        [Command("logPersonalTalk")]
        [Aliases("logTalk", "lt")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task LogTalk(CommandContext ctx, DiscordMember member, [RemainingText] string comments)
        {
            var response = await Mediator.Send(new LogTalkCommand { Comments = comments, UserId = member.Id }, ctx);

            await Mediator.RespondAsync(ctx, $"1 on 1 talk with {member.DisplayName} has been logged with id {response}", true);
        }

        [Command("logWarning")]
        [Aliases("lw")]
        [RequireRoles(RoleCheckMode.All, "Staff")]
        public async Task LogWarning(CommandContext ctx, DiscordMember member, [RemainingText] string comments)
        {
            var id = await Mediator.Send(new LogWarningCommand { Comments = comments, UserId = member.Id }, ctx);
            await Mediator.RespondAsync(ctx, $"Verbal warning logged with id {id}", true);
        }

        [Command("mute")]
        [RequireRoles(RoleCheckMode.All, "Staff")]
        public async Task Mute(CommandContext ctx, DiscordMember member, TimeSpan span, [RemainingText] string comments)
        {
            await Mediator.Send(new MuteCommand
            {
                Comments = comments,
                UserId = member.Id,
                UserDisplayName = member.DisplayName,
                TimeSpan = span,
                GuildId = ctx.Guild.Id
            }, ctx);

            await Mediator.RespondAsync(ctx, $"{member.DisplayName} has been muted", true);
        }

        [Command("mute")]
        public async Task MuteDefault(CommandContext ctx, DiscordMember member, [RemainingText] string comments)
        {
            await Mediator.Send(new MuteCommand
            {
                Comments = comments,
                UserId = member.Id,
                UserDisplayName = member.DisplayName,
                TimeSpan = TimeSpan.FromHours(1),
                GuildId = ctx.Guild.Id,
            }, ctx);
        }

        [Command("restrict")]
        [RequireRoles(RoleCheckMode.All, "Staff")]
        public async Task Restrict(CommandContext ctx, DiscordMember member, DiscordChannel channel, TimeSpan span,
            [RemainingText] string comments)
        {
            await Mediator.Send(new RestrictChannelCommand
            {
                Comments = comments,
                UserId = member.Id,
                TimeSpan = span,
                GuildId = ctx.Guild.Id,
                ChannelId = channel.Id,
                ChannelName = channel.Name,
            }, ctx);

            await Mediator.RespondAsync(ctx,
                $"{member.DisplayName}'s access to {channel.Mention} has been restricted for {span.Humanize()}",
                true);
        }

        [Command("restrict")]
        public async Task RestrictDefault(CommandContext ctx, DiscordMember member, DiscordChannel channel, [RemainingText] string comments)
        {
            var span = TimeSpan.FromHours(24);

            await Mediator.Send(new RestrictChannelCommand
            {
                Comments = comments,
                UserId = member.Id,
                TimeSpan = span,
                GuildId = ctx.Guild.Id,
                ChannelId = channel.Id,
                ChannelName = channel.Name,
            }, ctx);

            await Mediator.RespondAsync(ctx,
                $"{member.DisplayName}'s access to {channel.Mention} has been restricted for {span.Humanize()}",
                true);
        }
    }
}
