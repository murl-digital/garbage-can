using System;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Entities.Roles;
using GarbageCan.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GarbageCan.Application.Roles.JoinRoles.EventHandlers
{
    public class DiscordGuildMemberAddedHandler : INotificationHandler<DomainEventNotification<DiscordGuildMemberAdded>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DiscordGuildMemberAddedHandler> _logger;

        public DiscordGuildMemberAddedHandler(IApplicationDbContext context,
            ILogger<DiscordGuildMemberAddedHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Handle(DomainEventNotification<DiscordGuildMemberAdded> notification,
            CancellationToken cancellationToken)
        {
            try
            {
                if (await _context.JoinWatchlist.AnyAsync(x => x.UserId == notification.DomainEvent.UserId,
                    cancellationToken)) return;

                await _context.JoinWatchlist.AddAsync(new WatchedUser
                {
                    GuildId = notification.DomainEvent.GuildId,
                    UserId = notification.DomainEvent.UserId
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error during add of Join Watch list {@Notification}",
                    notification.DomainEvent);
            }
        }
    }
}
