using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordGuildChannelService
    {
        Task RenameChannel(ulong guildId, ulong channelId, string name);
    }
}
