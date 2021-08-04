using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Boosters.Commands
{
    public class PopulateBoosterServiceCommand : IRequest
    {
    }

    public class PopulateBoosterServiceCommandHandler : IRequestHandler<PopulateBoosterServiceCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBoosterService _boosterService;

        public PopulateBoosterServiceCommandHandler(IApplicationDbContext context, IBoosterService boosterService)
        {
            _context = context;
            _boosterService = boosterService;
        }

        public async Task<Unit> Handle(PopulateBoosterServiceCommand request, CancellationToken cancellationToken)
        {
            var availableSlots = await _context.XPAvailableSlots
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
            var queuedBoosters = await _context.XPQueuedBoosters
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
            var activeBoosters = await _context.XPActiveBoosters
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            _boosterService.AvailableSlots = availableSlots
                .GroupBy(s => s.GuildId)
                .ToDictionary(s => s.Key, s => s.ToList());
            _boosterService.QueuedBoosters = queuedBoosters
                .GroupBy(b => b.GuildId)
                .ToDictionary(
                    k => k.Key,
                    v => new Queue<QueuedBooster>(v.OrderBy(b => b.Position).ToList()));
            _boosterService.ActiveBoosters = activeBoosters
                .GroupBy(b => b.GuildId)
                .ToDictionary(k => k.Key, v => v.ToList());

            foreach (var key in _boosterService.AvailableSlots.Keys)
            {
                _boosterService.QueuedBoosters.TryAdd(key, new Queue<QueuedBooster>());
                _boosterService.ActiveBoosters.TryAdd(key, new List<ActiveBooster>());
            }

            return Unit.Value;
        }
    }
}
