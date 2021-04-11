using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordGuild
    {
        Task<DiscordMember> GetMemberAsync(ulong userId);
    }
}