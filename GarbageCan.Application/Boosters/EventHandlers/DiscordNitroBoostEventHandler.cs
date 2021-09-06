using System;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Boosters.Commands;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Events;
using MediatR;

namespace GarbageCan.Application.Boosters.EventHandlers
{
    public class DiscordNitroBoostEventHandler : INotificationHandler<DomainEventNotification<DiscordGuildUpdatedEvent>>
    {
        private readonly IMediator _mediator;
        private readonly IRngService _rngService;

        public DiscordNitroBoostEventHandler(IMediator mediator, IRngService rngService)
        {
            _mediator = mediator;
            _rngService = rngService;
        }

        public async Task Handle(DomainEventNotification<DiscordGuildUpdatedEvent> notification,
            CancellationToken cancellationToken)
        {
            if (notification.DomainEvent.BoostCount > notification.DomainEvent.PreviousBoostCount)
                await _mediator.Send(new AddBoosterCommand
                {
                    GuildId = notification.DomainEvent.GuildId,
                    Multiplier = (float)Math.Round(_rngService.FloatFromRange(1, 2), 2),
                    Duration = TimeSpan.FromMinutes(90),
                    Queue = false
                }, cancellationToken);
        }
    }
}
