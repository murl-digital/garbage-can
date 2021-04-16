using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Enums;
using GarbageCan.Domain.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Web.EventHandlers
{
    public class DiscordConnectionChangeShutdownHandler : INotificationHandler<DomainEventNotification<DiscordConnectionChangeEvent>>
    {
        private readonly DiscordClient _client;

        public DiscordConnectionChangeShutdownHandler(DiscordClient client)
        {
            _client = client;
        }

        public async Task Handle(DomainEventNotification<DiscordConnectionChangeEvent> notification, CancellationToken cancellationToken)
        {
            if (notification.DomainEvent.Status == DiscordConnectionStatus.Shutdown)
            {
                await _client.UpdateStatusAsync(null, UserStatus.Offline);
            }
        }
    }
}