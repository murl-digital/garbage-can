using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Models;
using GarbageCan.Application.XP.Commands.AddXpToUser;
using GarbageCan.Domain.Events;
using MediatR;

namespace GarbageCan.Application.XP.EventHandlers
{
    public class DiscordMessageCreatedEventHandler : INotificationHandler<DomainEventNotification<DiscordMessageCreatedEvent>>
    {
        private readonly IMediator _mediator;

        public DiscordMessageCreatedEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(DomainEventNotification<DiscordMessageCreatedEvent> notification, CancellationToken cancellationToken)
        {
            await _mediator.Send(new AddXpToUserCommand
            {
                UserId = notification.DomainEvent.AuthorId,
                Message = notification.DomainEvent.Content
            }, cancellationToken);
        }
    }
}