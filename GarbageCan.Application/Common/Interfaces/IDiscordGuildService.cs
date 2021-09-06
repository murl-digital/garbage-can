using System.Collections.Generic;
using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordGuildService
    {
        IEnumerable<ulong> GetAllCurrentGuilds();
    }
}
