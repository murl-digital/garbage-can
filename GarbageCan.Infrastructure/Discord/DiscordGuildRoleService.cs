using System.Collections.Generic;
using System.Linq;
using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Application.Common.Interfaces;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordGuildRoleService : IDiscordGuildRoleService
    {
        private readonly DiscordClient _client;
        private readonly DiscordGuildService _guildService;

        public DiscordGuildRoleService(DiscordClient client, DiscordGuildService guildService)
        {
            _client = client;
            _guildService = guildService;
        }

        public async Task GrantRoleAsync(ulong? guildId, ulong roleId, ulong userId, string reason = null)
        {
            var (member, role) = await GetRoleAndMember(guildId, roleId, userId);
            await member.GrantRoleAsync(role, reason);
        }

        public async Task RevokeRoleAsync(ulong? guildId, ulong roleId, ulong userId, string reason = null)
        {
            var (member, role) = await GetRoleAndMember(guildId, roleId, userId);
            await member.RevokeRoleAsync(role, reason);
        }

        public Task<Dictionary<ulong, Dictionary<ulong, ulong[]>>> GetAllMembersAndRoles()
        {
            var guildWithMembersAndRolesDictionary = _client.Guilds.Values.Select(x => new
                {
                    GuildId = x.Id,
                    MembersWithRoles = x.Members
                        .ToDictionary(k => k.Key, v => v.Value.Roles.Select(r => r.Id).ToArray())
                })
                .ToDictionary(x => x.GuildId, y => y.MembersWithRoles);
            return Task.FromResult(guildWithMembersAndRolesDictionary);
        }

        private async Task<(DiscordMember member, DiscordRole role)> GetRoleAndMember(ulong? guildId, ulong roleId, ulong userId)
        {
            var guild = await _guildService.GetGuild(guildId);
            var role = guild.GetRole(roleId);
            var member = await guild.GetMemberAsync(userId);
            return (member, role);
        }
    }
}
