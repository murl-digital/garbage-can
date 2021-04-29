using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Application.Common.Interfaces;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordModerationService : IDiscordModerationService
    {
        private readonly DiscordClient _client;

        public DiscordModerationService(DiscordClient client)
        {
            _client = client;
        }

        public async Task BanAsync(ulong guildId, ulong userId, string reason = null)
        {
            var member = await GetRole(guildId, userId);
            await member.BanAsync(reason: reason);
        }

        private async Task<DiscordMember> GetRole(ulong guildId, ulong userId)
        {
            var guild = await _client.GetGuildAsync(guildId);
            var member = await guild.GetMemberAsync(userId);
            return member;
        }
    }
}