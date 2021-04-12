using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordGuildService
    {
        Task<string> GetMemberDisplayNameAsync(ulong userId);
    }
}