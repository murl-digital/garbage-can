using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordGuildRoleService
    {
        Task GrantRoleAsync(ulong guildId, ulong roleId, ulong userId, string reason = null);
    }
}