using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Models;
using GarbageCan.Application.XP.Commands.CreateUser;
using GarbageCan.Domain.Events;
using MediatR;

namespace GarbageCan.Application.XP.EventHandlers
{
    public class DiscordGuildMemberAddedHandler : INotificationHandler<DomainEventNotification<DiscordGuildMemberAdded>>
    {
        private readonly IMediator _mediator;

        public DiscordGuildMemberAddedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(DomainEventNotification<DiscordGuildMemberAdded> notification, CancellationToken cancellationToken)
        {
            await _mediator.Send(new CreateXPUserCommand
            {
                UserId = notification.DomainEvent.UserId,
                IsBot = notification.DomainEvent.IsBot
            }, cancellationToken);
        }
    }
}