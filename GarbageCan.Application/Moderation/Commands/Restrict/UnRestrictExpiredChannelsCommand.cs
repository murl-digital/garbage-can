using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Moderation.Commands.Restrict
{
    public class UnRestrictExpiredChannelsCommand : IRequest
    {
        public ulong? GuildId { get; set; }
    }

    public class UnRestrictExpiredChannelsCommandHandler : IRequestHandler<UnRestrictExpiredChannelsCommand>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IDiscordModerationService _moderationService;
        private readonly IDiscordConfiguration _discordConfiguration;
        private readonly IDateTime _dateTime;
        private readonly ILogger<UnRestrictExpiredChannelsCommandHandler> _logger;

        public UnRestrictExpiredChannelsCommandHandler(IApplicationDbContext dbContext,
            IDiscordModerationService moderationService,
            IDiscordConfiguration discordConfiguration,
            IDateTime dateTime,
            ILogger<UnRestrictExpiredChannelsCommandHandler> logger)
        {
            _dbContext = dbContext;
            _moderationService = moderationService;
            _discordConfiguration = discordConfiguration;
            _dateTime = dateTime;
            _logger = logger;
        }

        public async Task<Unit> Handle(UnRestrictExpiredChannelsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Checking for expired channel restrictions");
            var utcNow = _dateTime.Now.ToUniversalTime();
            var expired = await _dbContext.ModerationActiveChannelRestricts
                .Where(c => c.expirationDate <= utcNow)
                .ToListAsync(cancellationToken);

            if (!expired.Any())
            {
                _logger.LogDebug("No for expired channel restrictions found");
                return Unit.Value;
            }

            _logger.LogInformation("Removing channel restrictions: {@Restrictions}", expired);

            foreach (var restrict in expired)
            {
                await _moderationService.RestoreChannelAccess(request.GuildId,
                    restrict.uId,
                    restrict.channelId,
                    "channel restrict expired");
            }

            _dbContext.ModerationActiveChannelRestricts.RemoveRange(expired);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
