using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;
using Z.EntityFramework.Plus;

namespace GarbageCan.Application.Boosters.Commands
{
    public class SaveQueueCommand : IRequest
    {
        public ulong GuildId { get; set; }
    }

    public class SaveQueueCommandHandler : IRequestHandler<SaveQueueCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBoosterService _boosterService;

        public SaveQueueCommandHandler(IApplicationDbContext context, IBoosterService boosterService)
        {
            _context = context;
            _boosterService = boosterService;
        }

        public async Task<Unit> Handle(SaveQueueCommand request, CancellationToken cancellationToken)
        {
            await _context.XPQueuedBoosters
                .Where(b => b.GuildId == request.GuildId)
                .DeleteAsync(cancellationToken);

            uint position = 0;
            foreach (var booster in _boosterService.QueuedBoosters[request.GuildId])
            {
                _context.XPQueuedBoosters.Add(new QueuedBooster
                {
                    GuildId = request.GuildId,
                    Multiplier = booster.Multiplier,
                    DurationInSeconds = booster.DurationInSeconds,
                    Position = position
                });
                position++;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
