using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Application.Common.Interfaces;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordGuildRoleService : IDiscordGuildRoleService
    {
        private readonly DiscordClient _client;

        public DiscordGuildRoleService(DiscordClient client)
        {
            _client = client;
        }

        public async Task GrantRoleAsync(ulong guildId, ulong roleId, ulong userId, string reason = null)
        {
            var (member, role) = await GetRoleAndMember(guildId, roleId, userId);
            await member.GrantRoleAsync(role, reason);
        }

        public async Task RevokeRoleAsync(ulong guildId, ulong roleId, ulong userId, string reason = null)
        {
            var (member, role) = await GetRoleAndMember(guildId, roleId, userId);
            await member.RevokeRoleAsync(role, reason);
        }

        private async Task<(DiscordMember member, DiscordRole role)> GetRoleAndMember(ulong guildId, ulong roleId, ulong userId)
        {
            var guild = await _client.GetGuildAsync(guildId);
            var role = guild.GetRole(roleId);
            var member = await guild.GetMemberAsync(userId);
            return (member, role);
        }
    }
}