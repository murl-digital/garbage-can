using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GarbageCan.Application.Fun.Commands.RandomStatus
{
    public class RandomizeStatusCommand : IRequest
    {
    }

    public class RandomizeStatusCommandHandler : IRequestHandler<RandomizeStatusCommand>
    {
        private readonly IDiscordConfiguration _configuration;
        private readonly IRngService _rngService;
        private readonly IDiscordPresenceService _presenceService;
        private readonly ILogger<RandomizeStatusCommandHandler> _logger;
        private readonly IApplicationDbContext _context;

        public RandomizeStatusCommandHandler(IDiscordConfiguration configuration, IRngService rngService,
            IDiscordPresenceService presenceService, ILogger<RandomizeStatusCommandHandler> logger,
            IApplicationDbContext context)
        {
            _configuration = configuration;
            _rngService = rngService;
            _context = context;
            _presenceService = presenceService;
            _logger = logger;
        }

        public async Task<Unit> Handle(RandomizeStatusCommand request, CancellationToken cancellationToken)
        {
            var prefix = _configuration.CommandPrefix;
            var dbList = await _context.CustomStatuses.ToArrayAsync(cancellationToken);
            var activity = dbList[_rngService.IntFromRange(0, dbList.Length)];
            _logger.LogDebug("i choose {Name}", activity.Name);

            await _presenceService.ChangeStatusAsync($"{activity.Name} | {prefix}help", activity.Activity);

            return Unit.Value;
        }
    }
}
