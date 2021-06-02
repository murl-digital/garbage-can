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

namespace GarbageCan.Application.Roles.EventHandlers
{
    public class DiscordGuildMemberAddedWatchListHandler : INotificationHandler<DomainEventNotification<DiscordGuildMemberAdded>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DiscordGuildMemberAddedWatchListHandler> _logger;

        public DiscordGuildMemberAddedWatchListHandler(IApplicationDbContext context, ILogger<DiscordGuildMemberAddedWatchListHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Handle(DomainEventNotification<DiscordGuildMemberAdded> notification, CancellationToken cancellationToken)
        {
            try
            {
                if (await _context.JoinWatchlist.AnyAsync(x => x.id == notification.DomainEvent.UserId, cancellationToken))
                {
                    return;
                }

                await _context.JoinWatchlist.AddAsync(new WatchedUser
                {
                    id = notification.DomainEvent.UserId
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error during add of Join Watch list {@Notification}", notification.DomainEvent);
            }
        }
    }
}