using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Application.Common.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordModerationService : IDiscordModerationService
    {
        private readonly DiscordGuildService _guildService;

        public DiscordModerationService(DiscordGuildService guildService)
        {
            _guildService = guildService;
        }

        public async Task BanAsync(ulong guildId, ulong userId, string reason = null)
        {
            var member = await GetMember(guildId, userId);
            await member.BanAsync(reason: reason);
        }

        public async Task RestoreChannelAccess(ulong? guildId, ulong userId, ulong channelId, string reason = null)
        {
            var member = await GetMember(guildId, userId);
            var channel = await GetChannel(guildId, channelId);

            var permissionOverwrites = channel.PermissionOverwrites.FirstOrDefault(o => o.Id == member.Id);
            if (permissionOverwrites != null)
            {
                await permissionOverwrites.DeleteAsync("channel restrict expired");
            }
        }

        public async Task RestrictChannelAccess(ulong guildId, ulong userId, ulong channelId)
        {
            var member = await GetMember(guildId, userId);
            var channel = await GetChannel(guildId, channelId);
            await channel.AddOverwriteAsync(member, Permissions.None, Permissions.AccessChannels);
        }

        private async Task<DiscordMember> GetMember(ulong? guildId, ulong userId)
        {
            var guild = await _guildService.GetGuild(guildId);
            var member = await guild.GetMemberAsync(userId);
            return member;
        }

        private async Task<DiscordChannel> GetChannel(ulong? guildId, ulong channelId)
        {
            var guild = await _guildService.GetGuild(guildId);
            var channel = guild.GetChannel(channelId);
            return channel;
        }
    }
}
