using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Application.Common.Interfaces;
using System.Threading.Tasks;

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

        public async Task RestrictChannelAccess(ulong guildId, ulong userId, ulong channelId)
        {
            var member = await GetRole(guildId, userId);
            var channel = await _client.GetChannelAsync(channelId);
            await channel.AddOverwriteAsync(member, Permissions.None, Permissions.AccessChannels);
        }

        private async Task<DiscordMember> GetRole(ulong guildId, ulong userId)
        {
            var guild = await _client.GetGuildAsync(guildId);
            var member = await guild.GetMemberAsync(userId);
            return member;
        }
    }
}