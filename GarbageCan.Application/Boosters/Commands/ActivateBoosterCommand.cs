using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;

namespace GarbageCan.Application.Boosters.Commands
{
    public class ActivateBoosterCommand : IRequest
    {
        public ulong GuildId { get; set; }
        public float Multiplier { get; set; }
        public TimeSpan Duration { get; set; }
        public AvailableSlot Slot { get; set; }
    }

    public class ActivateBoosterCommandHandler : IRequestHandler<ActivateBoosterCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBoosterService _boosterService;
        private readonly IDateTime _dateTime;

        public ActivateBoosterCommandHandler(IApplicationDbContext context, IBoosterService boosterService,
            IDateTime dateTime)
        {
            _context = context;
            _boosterService = boosterService;
            _dateTime = dateTime;
        }

        public async Task<Unit> Handle(ActivateBoosterCommand request, CancellationToken cancellationToken)
        {
            if (!_boosterService.AvailableSlots.ContainsKey(request.GuildId))
                throw new InvalidOperationException("Specified guild has no available slots");

            if (_boosterService.AvailableSlots[request.GuildId].All(s => s.Id == request.Slot.Id))
                throw new InvalidOperationException("Specified slot doesn't exist in guild");

            var booster = new ActiveBooster
            {
                GuildId = request.GuildId,
                ExpirationDate = _dateTime.Now.ToUniversalTime().Add(request.Duration),
                Multiplier = request.Multiplier,
                Slot = request.Slot
            };

            _boosterService.ActiveBoosters[request.GuildId].Add(booster);

            _context.XPActiveBoosters.Add(booster);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
