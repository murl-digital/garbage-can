using System.Collections.Generic;
using DSharpPlus;
using GarbageCan.Application.Common.Interfaces;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordGuildService : IDiscordGuildService
    {
        private readonly DiscordClient _client;

        public DiscordGuildService(DiscordClient client)
        {
            _client = client;
        }

        public IEnumerable<ulong> GetAllCurrentGuilds()
        {
            return _client.Guilds.Keys;
        }
    }
}
