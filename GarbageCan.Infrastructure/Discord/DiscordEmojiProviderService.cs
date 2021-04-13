using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Application.Common.Interfaces;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordEmojiProviderService
    {
        private readonly DiscordClient _client;
        private readonly IDiscordConfiguration _configuration;
        private DiscordEmoji _discordEmoji;

        public DiscordEmojiProviderService(DiscordClient client, IDiscordConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public string GetEmoji()
        {
            if (_discordEmoji == null)
            {
                _discordEmoji = DiscordEmoji.FromName(_client, _configuration.EmojiName);
            }

            return _discordEmoji;
        }
    }
}