using System.Linq;
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
        private readonly IApplicationDbContext _context;

        public DiscordMessageCreatedEventHandler(IMediator mediator, IDiscordConfiguration configuration, IApplicationDbContext context)
        {
            _mediator = mediator;
            _configuration = configuration;
            _context = context;
        }

        public async Task Handle(DomainEventNotification<DiscordMessageCreatedEvent> notification,
            CancellationToken cancellationToken)
        {
            if (notification.DomainEvent.AuthorIsBot ||
                notification.DomainEvent.AuthorIsSystem ||
                notification.DomainEvent.ChannelIsPrivate ||
                notification.DomainEvent.Content.StartsWith(_configuration.CommandPrefix) ||
                _context.XPExcludedChannels.Any(c => c.GuildId == notification.DomainEvent.GuildId && c.ChannelId == notification.DomainEvent.ChannelId))
            {
                return;
            }

            await _mediator.Send(new AddXpToUserCommand
            {
                GuildId = notification.DomainEvent.GuildId,
                ChannelId = notification.DomainEvent.ChannelId,
                UserId = notification.DomainEvent.AuthorId,
                Message = notification.DomainEvent.Content,
                UserAvatarUrl = notification.DomainEvent.AuthorAvatarUrl,
                UserDisplayName = notification.DomainEvent.AuthorDisplayName,
            }, cancellationToken);
        }
    }
}
