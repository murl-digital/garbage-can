using System.Threading.Tasks;
using GarbageCan.Domain.Enums;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordPresenceService
    {
        public Task ChangeStatusAsync(string name, Activity activity);
    }
}
