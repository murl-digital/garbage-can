using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Z.EntityFramework.Plus;

namespace GarbageCan.Application.Boosters.Commands
{
    public class UpdateBoosterCycleCommand : IRequest
    {
        public ulong GuildId { get; set; }
    }

    public class UpdateBoosterCycleCommandHandler : IRequestHandler<UpdateBoosterCycleCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBoosterService _boosterService;
        private readonly IDateTime _dateTime;
        private readonly IDiscordGuildChannelService _discordChannelService;
        private readonly IMediator _mediator;

        public UpdateBoosterCycleCommandHandler(IApplicationDbContext context, IBoosterService boosterService,
            IDateTime dateTime, IDiscordGuildChannelService discordChannelService, IMediator mediator)
        {
            _context = context;
            _boosterService = boosterService;
            _dateTime = dateTime;
            _mediator = mediator;
            _discordChannelService = discordChannelService;
        }

        public async Task<Unit> Handle(UpdateBoosterCycleCommand request, CancellationToken cancellationToken)
        {
            var now = _dateTime.Now.ToUniversalTime();

            _boosterService.ActiveBoosters[request.GuildId]
                .RemoveAll(b => b.ExpirationDate < now);
            await _context.XPActiveBoosters
                .Where(b => b.ExpirationDate < now)
                .DeleteAsync(cancellationToken);

            var saveQueue = false;

            while (_boosterService.QueuedBoosters[request.GuildId].Count > 0 &&
                   _boosterService.ActiveBoosters[request.GuildId].Count <
                   _boosterService.AvailableSlots[request.GuildId].Count)
            {
                saveQueue = true;

                var usedSlots = _boosterService.ActiveBoosters[request.GuildId]
                    .Select(b => b.Slot)
                    .ToList();

                var slot = _boosterService.AvailableSlots[request.GuildId]
                    .First(s => !usedSlots.Contains(s));

                var booster = _boosterService.QueuedBoosters[request.GuildId].Dequeue();

                await _mediator.Send(new ActivateBoosterCommand
                {
                    GuildId = request.GuildId,
                    Slot = slot,
                    Multiplier = booster.Multiplier,
                    Duration = TimeSpan.FromSeconds(booster.DurationInSeconds)
                }, cancellationToken);
            }

            if (_boosterService.ActiveBoosters[request.GuildId].Count <
                _boosterService.AvailableSlots[request.GuildId].Count)
            {
                var usedSlots = _boosterService.ActiveBoosters[request.GuildId]
                    .Select(b => b.Slot)
                    .ToList();

                foreach (var slot in _boosterService.AvailableSlots[request.GuildId].Where(s => !usedSlots.Contains(s)))
                    await _discordChannelService.RenameChannel(request.GuildId, slot.ChannelId, "-");
            }

            if (saveQueue) await _mediator.Send(new SaveQueueCommand { GuildId = request.GuildId }, cancellationToken);

            return Unit.Value;
        }
    }
}
