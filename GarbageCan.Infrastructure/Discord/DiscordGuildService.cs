using GarbageCan.Application.Common.Interfaces;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordGuildService : IDiscordGuildService
    {
        public Task<string> GetMemberDisplayNameAsync(ulong userId)
        {
            // TODO: This is just for now
            return Task.FromResult("TEST");
        }
    }
}