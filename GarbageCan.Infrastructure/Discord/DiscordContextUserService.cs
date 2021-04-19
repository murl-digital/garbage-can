using GarbageCan.Application.Common.Interfaces;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordContextUserService : ICurrentUserService
    {
        private readonly DiscordCommandContextService _contextService;

        public DiscordContextUserService(DiscordCommandContextService contextService)
        {
            _contextService = contextService;
        }

        public string UserId => _contextService.CommandContext?.User?.Id.ToString();
        public string DisplayName => _contextService.CommandContext?.Member?.DisplayName;
    }
}