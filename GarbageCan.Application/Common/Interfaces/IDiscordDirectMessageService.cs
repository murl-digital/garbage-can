using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordDirectMessageService
    {
        Task SendMessageAsync(ulong userId, string message);
    }
}