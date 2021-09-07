using System.Collections.Generic;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Models;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordGuildMemberService
    {
        List<Member> GetGuildMembers(ulong? guildId);
        Task<Member> GetMemberAsync(ulong? guildId, ulong id);
    }
}
