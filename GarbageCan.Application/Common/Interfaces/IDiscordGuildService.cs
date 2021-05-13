using System.Collections.Generic;
using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordGuildService
    {
        Task<Dictionary<ulong, string>> GetChannelNamesById(IEnumerable<ulong> channelIds);

        Task<string> GetMemberDisplayNameAsync(ulong userId);

        Task<Dictionary<ulong, string>> GetRoleNamesById(IEnumerable<ulong> roleIds);
    }
}