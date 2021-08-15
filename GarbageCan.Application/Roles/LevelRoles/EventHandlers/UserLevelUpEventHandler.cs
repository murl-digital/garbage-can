using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Models;
using GarbageCan.Application.Roles.LevelRoles.Commands.ApplyLevelRoles;
using GarbageCan.Domain.Events;
using MediatR;

namespace GarbageCan.Application.Roles.LevelRoles.EventHandlers
{
    public class UserLevelUpEventHandler : INotificationHandler<DomainEventNotification<UserLevelUpEvent>>
    {
        private readonly IMediator _mediator;

        public UserLevelUpEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(DomainEventNotification<UserLevelUpEvent> notification, CancellationToken cancellationToken)
        {
            var levels = Enumerable.Range(notification.DomainEvent.OldLvl + 1,
                Math.Abs(notification.DomainEvent.NewLvl - notification.DomainEvent.OldLvl));

            foreach (var level in levels)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await _mediator.Send(new ApplyLevelRolesCommand
                {
                    GuildId = notification.DomainEvent.GuildId,
                    MemberId = notification.DomainEvent.UserId,
                    Level = level
                }, cancellationToken);
            }
        }
    }
}
