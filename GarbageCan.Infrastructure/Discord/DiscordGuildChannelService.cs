using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordGuildChannelService : IDiscordGuildChannelService
    {
        private readonly DiscordGuildService _guildService;

        public DiscordGuildChannelService(DiscordGuildService guildService)
        {
            _guildService = guildService;
        }

        public async Task RenameChannel(ulong guildId, ulong channelId, string name)
        {
            var guild = await _guildService.GetGuild(guildId);
            await guild.GetChannel(channelId).ModifyAsync(channel => channel.Name = name);
        }
    }
}
