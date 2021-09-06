using System.Threading.Tasks;
using DSharpPlus;
using GarbageCan.Application.Common.Interfaces;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordGuildChannelService : IDiscordGuildChannelService
    {
        private readonly DiscordClient _discordClient;

        public DiscordGuildChannelService(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task RenameChannel(ulong guildId, ulong channelId, string name)
        {
            await _discordClient.Guilds[guildId].GetChannel(channelId).ModifyAsync(channel => channel.Name = name);
        }
    }
}
