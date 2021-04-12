using GarbageCan.Application.Common.Models;
using GarbageCan.Application.Services;
using GarbageCan.Domain.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Discord.EventHandlers
{
    public class ConnectionStatusChangeHandler : INotificationHandler<DomainEventNotification<DiscordConnectionChangeEvent>>
    {
        private readonly DiscordConnectionService _service;

        public ConnectionStatusChangeHandler(DiscordConnectionService service)
        {
            _service = service;
        }

        public Task Handle(DomainEventNotification<DiscordConnectionChangeEvent> notification, CancellationToken cancellationToken)
        {
            _service.Status = notification.DomainEvent.Status;
            return Task.CompletedTask;
        }
    }
}