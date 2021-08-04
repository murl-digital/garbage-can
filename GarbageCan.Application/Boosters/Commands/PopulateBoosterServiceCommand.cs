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
            _boosterService.AvailableSlots = await _context.XPAvailableSlots
                .AsNoTracking()
                .GroupBy(s => s.GuildId)
                .ToDictionaryAsync(s => s.Key, s => s.ToList(), cancellationToken);
            _boosterService.QueuedBoosters = await _context.XPQueuedBoosters
                .AsNoTracking()
                .GroupBy(b => b.GuildId)
                .ToDictionaryAsync(
                    k => k.Key,
                    v => new Queue<QueuedBooster>(v.OrderBy(b => b.Position).ToList()),
                    cancellationToken);
            _boosterService.ActiveBoosters = await _context.XPActiveBoosters
                .AsNoTracking()
                .GroupBy(b => b.GuildId)
                .ToDictionaryAsync(k => k.Key, v => v.ToList(), cancellationToken);

            return Unit.Value;
        }
    }
}
