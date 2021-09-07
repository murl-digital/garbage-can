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
        private readonly DiscordCommandContextService _contextService;

        public DiscordGuildService(DiscordClient client, DiscordCommandContextService contextService)
        {
            _client = client;
            _contextService = contextService;
        }

        public IEnumerable<ulong> GetAllCurrentGuildIds()
        {
            return _client.Guilds.Keys;
        }

        public IEnumerable<DiscordGuild> GetAllCurrentGuilds()
        {
            return _client.Guilds.Values;
        }

        public Task<DiscordGuild> GetGuild(ulong? guildId)
        {
            var hasContext = _contextService.CommandContext != null;
            var guild = guildId.HasValue
                ? _client.Guilds[guildId.Value]
                : hasContext ? _contextService.CommandContext.Guild : _client.Guilds.Values.FirstOrDefault();
            return Task.FromResult(guild);
        }
    }
}
