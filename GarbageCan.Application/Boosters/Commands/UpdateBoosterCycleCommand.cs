using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
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
            var (activeBoosters, queuedBoosters, availableSlots) = GetTheDeets(request.GuildId);
            var now = _dateTime.Now.ToUniversalTime();

            activeBoosters
                .RemoveAll(b => b.ExpirationDate < now);
            await _context.XPActiveBoosters
                .Where(b => b.ExpirationDate < now)
                .DeleteAsync(cancellationToken);

            var saveQueue = false;

            while (queuedBoosters.Count > 0 && activeBoosters.Count < availableSlots.Count)
            {
                saveQueue = true;

                var usedSlots = activeBoosters
                    .Select(b => b.Slot.Id)
                    .ToList();

                var slot = availableSlots
                    .First(s => !usedSlots.Contains(s.Id));

                var booster = queuedBoosters.Dequeue();

                await _mediator.Send(new ActivateBoosterCommand
                {
                    GuildId = request.GuildId,
                    Slot = slot,
                    Multiplier = booster.Multiplier,
                    Duration = TimeSpan.FromSeconds(booster.DurationInSeconds)
                }, cancellationToken);
            }

            if (activeBoosters.Count < availableSlots.Count)
            {
                var usedSlots = activeBoosters
                    .Select(b => b.Slot.Id)
                    .ToList();

                foreach (var slot in availableSlots
                    .Where(s => !usedSlots.Contains(s.Id)))
                    await _discordChannelService.RenameChannel(request.GuildId, slot.ChannelId, "-");
            }

            if (saveQueue) await _mediator.Send(new SaveQueueCommand { GuildId = request.GuildId }, cancellationToken);

            return Unit.Value;
        }

        private (List<ActiveBooster>, Queue<QueuedBooster>, List<AvailableSlot>)
            GetTheDeets(ulong guildId)
        {
            return (_boosterService.ActiveBoosters[guildId], _boosterService.QueuedBoosters[guildId],
                _boosterService.AvailableSlots[guildId]);
        }
    }
}
