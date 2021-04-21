using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Events;
using MediatR;

namespace GarbageCan.Application.XP.EventHandlers
{
    public class DiscordMessageCreatedEventHandler : INotificationHandler<DomainEventNotification<DiscordMessageCreatedEvent>>
    {
        public Task Handle(DomainEventNotification<DiscordMessageCreatedEvent> notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}