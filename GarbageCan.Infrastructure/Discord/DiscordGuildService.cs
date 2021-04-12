using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Infrastructure.Discord.Exceptions;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordGuildService : IDiscordGuildService
    {
        private readonly DiscordCommandContextService _contextService;

        public DiscordGuildService(DiscordCommandContextService contextService)
        {
            _contextService = contextService;
        }

        public async Task<string> GetMemberDisplayNameAsync(ulong userId)
        {
            if (_contextService.CommandContext != null)
            {
                var member = await _contextService.CommandContext.Guild.GetMemberAsync(userId);
                return member?.DisplayName;
            }

            throw new CommandContextMissingException();
        }
    }
}