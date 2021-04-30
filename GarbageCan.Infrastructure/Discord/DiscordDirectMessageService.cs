using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordDirectMessageService : IDiscordDirectMessageService
    {
        private readonly DiscordClient _contextService;
        private readonly IDiscordConfiguration _discordConfiguration;
        private readonly ILogger<DiscordDirectMessageService> _logger;

        public DiscordDirectMessageService(DiscordClient contextService,
            IDiscordConfiguration discordConfiguration,
            ILogger<DiscordDirectMessageService> logger)
        {
            _contextService = contextService;
            _discordConfiguration = discordConfiguration;
            _logger = logger;
        }

        public async Task SendMessageAsync(ulong userId, string message)
        {
            try
            {
                var member = await _contextService.Guilds[_discordConfiguration.GuildId].GetMemberAsync(userId);
                try
                {
                    var channel = await member.CreateDmChannelAsync();
                    await channel.SendMessageAsync(message);
                }
                catch (UnauthorizedException)
                {
                    var hideOverwrite = new DiscordOverwriteBuilder()
                        .Deny(Permissions.AccessChannels)
                        .For(member.Guild.EveryoneRole);
                    var showOverwrite = new DiscordOverwriteBuilder()
                        .Allow(Permissions.AccessChannels)
                        .For(member);
                    var readonlyOverwrite = new DiscordOverwriteBuilder()
                        .Deny(Permissions.SendMessages)
                        .Deny(Permissions.AddReactions)
                        .For(member);
                    var channel = await member.Guild.CreateChannelAsync(
                        "message-" + member.Username,
                        ChannelType.Text,
                        null,
                        overwrites: new[] { hideOverwrite, showOverwrite, readonlyOverwrite });

                    await channel.SendMessageAsync(
                        "Hello, " + member
                            .Mention +
                        ". We tried to send you a direct message, however your direct messages are disabled for this server. Below is the message in question.");
                    await channel.SendMessageAsync(message);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Couldn't send message {Message} to User {UserId}", message, userId);
            }
        }
    }
}