using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordResponseService
    {
        Task RespondAsync(string message, bool prependEmoji = false, bool formatAsBlock = false);
    }
}