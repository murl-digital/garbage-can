using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;
using GarbageCan.Application.XP.Commands.AddXpToUser;
using GarbageCan.Domain.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.XP.EventHandlers
{
    public class DiscordMessageCreatedEventHandler : INotificationHandler<DomainEventNotification<DiscordMessageCreatedEvent>>
    {
        private readonly IMediator _mediator;
        private readonly IDiscordConfiguration _configuration;

        public DiscordMessageCreatedEventHandler(IMediator mediator, IDiscordConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        public async Task Handle(DomainEventNotification<DiscordMessageCreatedEvent> notification, CancellationToken cancellationToken)
        {
            if (notification.DomainEvent.Content.StartsWith(_configuration.CommandPrefix))
            {
                return;
            }

            await _mediator.Send(new AddXpToUserCommand
            {
                UserId = notification.DomainEvent.AuthorId,
                Message = notification.DomainEvent.Content
            }, cancellationToken);
        }
    }
}