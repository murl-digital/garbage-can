using DSharpPlus;
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
            var guild = await _client.GetGuildAsync(guildId);
            var role = guild.GetRole(roleId);
            var member = await guild.GetMemberAsync(userId);
            await member.GrantRoleAsync(role, reason);
        }
    }
}