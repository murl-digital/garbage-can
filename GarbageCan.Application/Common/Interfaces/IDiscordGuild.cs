using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordGuild
    {
        Task<string> GetMemberDisplayNameAsync(ulong userId);
    }
}