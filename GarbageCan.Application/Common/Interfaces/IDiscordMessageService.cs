using GarbageCan.Domain.Entities;
using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordMessageService
    {
        Task CreateReactionAsync(ulong channelId, ulong messageId, Emoji emoji);
    }
}