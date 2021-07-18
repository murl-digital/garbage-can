using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GarbageCan.Application.Roles.EventHandlers
{
    public class DiscordGuildMemberUpdatedHandler : INotificationHandler<DomainEventNotification<DiscordGuildMemberUpdated>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordGuildRoleService _roleService;
        private readonly ILogger<DiscordGuildMemberUpdatedHandler> _logger;

        public DiscordGuildMemberUpdatedHandler(IApplicationDbContext context, IDiscordGuildRoleService roleService, ILogger<DiscordGuildMemberUpdatedHandler> logger)
        {
            _context = context;
            _roleService = roleService;
            _logger = logger;
        }

        public async Task Handle(DomainEventNotification<DiscordGuildMemberUpdated> notification, CancellationToken cancellationToken)
        {
            if (notification.DomainEvent.IsBot)
            {
                return;
            }

            try
            {
                var watchListUsers = await _context.JoinWatchlist
                    .Where(x => x.id == notification.DomainEvent.UserId)
                    .ToListAsync(cancellationToken);

                if (watchListUsers.Any())
                {
                    watchListUsers.ForEach(x => _context.JoinWatchlist.Remove(x));

                    await _context.SaveChangesAsync(cancellationToken);

                    var joinRoles = await _context.JoinRoles.ToListAsync(cancellationToken);

                    foreach (var r in joinRoles)
                    {
                        try
                        {
                            await _roleService.GrantRoleAsync(notification.DomainEvent.GuildId,
                                r.roleId,
                                notification.DomainEvent.UserId,
                                "join role");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "couldn't grant role to user");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error during add of Join Watch list {@Notification}", notification.DomainEvent);
            }
        }
    }
}