using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Boosters.Commands;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Enums;
using GarbageCan.Domain.Events;
using MediatR;

namespace GarbageCan.Web.EventHandlers
{
    public class
        DiscordConnectionChangeHandler : INotificationHandler<DomainEventNotification<DiscordConnectionChangeEvent>>
    {
        private readonly IMediator _mediator;

        public DiscordConnectionChangeHandler(IMediator mediator)
        {
            _mediator = mediator;
        }


        public async Task Handle(DomainEventNotification<DiscordConnectionChangeEvent> notification,
            CancellationToken cancellationToken)
        {
            if (notification.DomainEvent.Status == DiscordConnectionStatus.Ready)
                await _mediator.Send(new PopulateBoosterServiceCommand(), cancellationToken);
        }
    }
}
