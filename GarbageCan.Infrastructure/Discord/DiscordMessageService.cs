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
        private readonly DiscordGuildService _guildService;
        private readonly DiscordClient _client;
        private readonly ILogger<DiscordMessageService> _logger;

        public DiscordMessageService(DiscordGuildService guildService,
            DiscordClient client,
            ILogger<DiscordMessageService> logger)
        {
            _guildService = guildService;
            _client = client;
            _logger = logger;
        }

        public async Task CreateReactionAsync(ulong? guildId, ulong channelId, ulong messageId, Emoji emoji)
        {
            var guild = await _guildService.GetGuild(guildId);

            try
            {
                var msg = await guild.GetChannel(channelId).GetMessageAsync(messageId);
                await msg.CreateReactionAsync(GetEmoji(emoji, _client));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't create reaction for message: {messageId} Channel: {channelId} Guild: {@guild}", messageId,
                    channelId, new { guild.Id, guild.Name });
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
