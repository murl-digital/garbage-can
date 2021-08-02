using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Models;
using GarbageCan.Application.Roles.Commands.AlterRole;
using GarbageCan.Domain.Events;
using MediatR;

namespace GarbageCan.Application.Roles.EventHandlers
{
    public class
        DiscordMessageReactionRemovedEventHandler : INotificationHandler<
            DomainEventNotification<DiscordMessageReactionRemovedEvent>>
    {
        private readonly IMediator _mediator;

        public DiscordMessageReactionRemovedEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(DomainEventNotification<DiscordMessageReactionRemovedEvent> notification,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new ApplyReactionRolesCommand
            {
                ChannelId = notification.DomainEvent.ChannelId,
                Emoji = notification.DomainEvent.Emoji,
                GuildId = notification.DomainEvent.GuildId,
                MessageId = notification.DomainEvent.MessageId,
                UserId = notification.DomainEvent.UserId,
                Add = false
            }, cancellationToken);
        }
    }
}
