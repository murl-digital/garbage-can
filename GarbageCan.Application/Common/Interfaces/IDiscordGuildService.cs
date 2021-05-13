using System.Collections.Generic;
using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordGuildService
    {
        Task<string> GetMemberDisplayNameAsync(ulong userId);

        Task<Dictionary<ulong, string>> GetRoleNamesById(IEnumerable<ulong> roleIds);
    }
}