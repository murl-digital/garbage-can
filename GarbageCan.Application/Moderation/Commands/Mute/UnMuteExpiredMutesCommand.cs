using GarbageCan.Application.Common.Configuration;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Moderation.Commands.Mute
{
    public class UnMuteExpiredMutesCommand : IRequest
    {
    }

    public class UnMuteExpiredMutesCommandHandler : IRequestHandler<UnMuteExpiredMutesCommand>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IDiscordGuildRoleService _roleService;
        private readonly IDiscordConfiguration _discordConfiguration;
        private readonly IRoleConfiguration _roleConfiguration;
        private readonly IDateTime _dateTime;
        private readonly ILogger _logger;

        public UnMuteExpiredMutesCommandHandler(IApplicationDbContext dbContext,
            IDiscordGuildRoleService roleService,
            IDiscordConfiguration discordConfiguration,
            IRoleConfiguration roleConfiguration,
            IDateTime dateTime,
            ILogger<UnMuteExpiredMutesCommandHandler> logger)
        {
            _dbContext = dbContext;
            _roleService = roleService;
            _discordConfiguration = discordConfiguration;
            _roleConfiguration = roleConfiguration;
            _dateTime = dateTime;
            _logger = logger;
        }

        public async Task<Unit> Handle(UnMuteExpiredMutesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Checking for expired mutes");
            var utcNow = _dateTime.Now.ToUniversalTime();
            var expiredMutes = await _dbContext.ModerationActiveMutes
                .Where(c => c.expirationDate <= utcNow)
                .ToListAsync(cancellationToken);

            if (!expiredMutes.Any())
            {
                _logger.LogDebug("No for expired mutes found");
                return Unit.Value;
            }

            _logger.LogInformation("Removing mutes: {@Mutes}", expiredMutes);

            foreach (var restrict in expiredMutes)
            {
                await _roleService.RevokeRoleAsync(_discordConfiguration.GuildId,
                   _roleConfiguration.MuteRoleId,
                    restrict.uId,
                    "mute expired");
            }

            _dbContext.ModerationActiveMutes.RemoveRange(expiredMutes);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}