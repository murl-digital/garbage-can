using System;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Models;
using GarbageCan.Application.XP.Commands.CreateUser;
using GarbageCan.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GarbageCan.Application.XP.EventHandlers
{
    public class DiscordGuildMemberAddedHandler : INotificationHandler<DomainEventNotification<DiscordGuildMemberAdded>>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DiscordGuildMemberAddedHandler> _logger;

        public DiscordGuildMemberAddedHandler(IMediator mediator, ILogger<DiscordGuildMemberAddedHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(DomainEventNotification<DiscordGuildMemberAdded> notification, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new CreateXPUserCommand
                {
                    GuildId = notification.DomainEvent.GuildId,
                    UserId = notification.DomainEvent.UserId,
                    IsBot = notification.DomainEvent.IsBot
                }, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error during XP User creation on Guild Member Add");
            }
        }
    }
}
