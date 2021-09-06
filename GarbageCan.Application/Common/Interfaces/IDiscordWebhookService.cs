using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordWebhookService
    {
        Task CreateUserWebhook(string title, string description, string hookName, string avatarUrl, ulong guildId, ulong channelId);
    }
}
