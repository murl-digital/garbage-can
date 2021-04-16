using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Infrastructure.Discord.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordGuildService : IDiscordGuildService
    {
        private readonly DiscordCommandContextService _contextService;
        private readonly ILogger<DiscordGuildService> _logger;

        public DiscordGuildService(DiscordCommandContextService contextService, ILogger<DiscordGuildService> logger)
        {
            _contextService = contextService;
            _logger = logger;
        }

        public async Task<string> GetMemberDisplayNameAsync(ulong userId)
        {
            if (_contextService.CommandContext == null)
            {
                throw new CommandContextMissingException();
            }

            var guild = _contextService.CommandContext.Guild;

            try
            {
                var member = await guild.GetMemberAsync(userId);
                return member?.DisplayName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't get guild for user: {userId} Guild: {@guild}", userId, new { guild.Id, guild.Name });
                throw;
            }
        }
    }
}