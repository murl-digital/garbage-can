using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using GarbageCan.Domain.Enums;
using MediatR;

namespace GarbageCan.Application.Boosters.Commands
{
    public class AddBoosterCommand : IRequest<BoosterResult>
    {
        public ulong GuildId { get; set; }
        public float Multiplier { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Queue { get; set; }
    }

    public class AddBoosterCommandHandler : IRequestHandler<AddBoosterCommand, BoosterResult>
    {
        private readonly IBoosterService _boosterService;
        private readonly IMediator _mediator;

        public AddBoosterCommandHandler(IBoosterService boosterService, IMediator mediator)
        {
            _boosterService = boosterService;
            _mediator = mediator;
        }

        public async Task<BoosterResult> Handle(AddBoosterCommand request, CancellationToken cancellationToken)
        {
            if (!_boosterService.AvailableSlots.ContainsKey(request.GuildId))
                throw new InvalidOperationException("Specified guild has no available slots");

            if (_boosterService.ActiveBoosters[request.GuildId].Count >=
                _boosterService.AvailableSlots[request.GuildId].Count)
            {
                if (!request.Queue) return BoosterResult.SlotsFull;

                _boosterService.QueuedBoosters[request.GuildId].Enqueue(new QueuedBooster
                {
                    Multiplier = request.Multiplier,
                    DurationInSeconds = (long)request.Duration.TotalSeconds
                });
                await _mediator.Send(new SaveQueueCommand
                {
                    GuildId = request.GuildId
                }, cancellationToken);
                return BoosterResult.Queued;
            }

            var usedSlots = _boosterService.ActiveBoosters[request.GuildId]
                .Select(b => b.Slot)
                .ToList();

            var slot = _boosterService.AvailableSlots[request.GuildId]
                .First(s => !usedSlots.Contains(s));

            await _mediator.Send(new ActivateBoosterCommand
            {
                GuildId = request.GuildId,
                Multiplier = request.Multiplier,
                Duration = request.Duration,
                Slot = slot
            }, cancellationToken);
            return BoosterResult.Active;
        }
    }
}
