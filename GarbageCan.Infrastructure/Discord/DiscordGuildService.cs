using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
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

        public Task<DiscordGuild> GetGuild(ulong? guildId)
        {
            var guild = guildId.HasValue ? _client.Guilds[guildId.Value] : _client.Guilds.Values.FirstOrDefault();
            return Task.FromResult(guild);
        }
    }
}
