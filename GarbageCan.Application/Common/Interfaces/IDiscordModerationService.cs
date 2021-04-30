using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordModerationService
    {
        Task BanAsync(ulong guildId, ulong userId, string reason = null);

        Task RestrictChannelAccess(ulong guildId, ulong userId, ulong channelId);
    }
}