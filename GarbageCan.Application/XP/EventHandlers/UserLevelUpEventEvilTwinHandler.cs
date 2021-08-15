using System;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GarbageCan.Application.XP.EventHandlers
{
    public class UserLevelUpEventEvilTwinHandler : INotificationHandler<DomainEventNotification<UserLevelUpEvent>>
    {
        private readonly IDiscordWebhookService _webhookService;
        private readonly ILogger<UserLevelUpEventEvilTwinHandler> _logger;

        public UserLevelUpEventEvilTwinHandler(IDiscordWebhookService webhookService,
            ILogger<UserLevelUpEventEvilTwinHandler> logger)
        {
            _webhookService = webhookService;
            _logger = logger;
        }


        public async Task Handle(DomainEventNotification<UserLevelUpEvent> notification, CancellationToken cancellationToken)
        {
            try
            {
                var details = notification.DomainEvent.MessageDetails;

                await _webhookService.CreateUserWebhook("Level up!",
                    $"Congrats to {details.UserDisplayName} for reaching level {notification.DomainEvent.NewLvl}!",
                    $"{details.UserDisplayName}'s evil clone",
                    details.UserAvatarUrl,
                    details.GuildId, 
                    details.ChannelId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't send Evil Twin message");
            }
        }
    }
}
