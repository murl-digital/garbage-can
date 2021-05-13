using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities;
using GarbageCan.Infrastructure.Discord.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordMessageService : IDiscordMessageService
    {
        private readonly DiscordCommandContextService _contextService;
        private readonly ILogger<DiscordMessageService> _logger;

        public DiscordMessageService(DiscordCommandContextService contextService, ILogger<DiscordMessageService> logger)
        {
            _contextService = contextService;
            _logger = logger;
        }

        public async Task CreateReactionAsync(ulong channelId, ulong messageId, Emoji emoji)
        {
            if (_contextService.CommandContext == null)
            {
                throw new CommandContextMissingException();
            }

            var guild = _contextService.CommandContext.Guild;

            try
            {
                var msg = await guild.GetChannel(channelId).GetMessageAsync(messageId);
                var client = _contextService.CommandContext.Client;
                await msg.CreateReactionAsync(GetEmoji(emoji, client));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't create reaction for message: {messageId} Channel: {channelId} Guild: {@guild}", messageId, channelId, new { guild.Id, guild.Name });
                throw;
            }
        }

        private static DiscordEmoji GetEmoji(Emoji emoji, DiscordClient client)
        {
            return emoji.Id == 0
                ? DiscordEmoji.FromUnicode(client, emoji.Name)
                : DiscordEmoji.FromGuildEmote(client, emoji.Id);
        }
    }
}