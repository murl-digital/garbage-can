using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;
using GarbageCan.Application.Roles.Commands.ApplyLevelRoles;
using GarbageCan.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.EventHandlers
{
    public class UserLevelUpEventHandler : INotificationHandler<DomainEventNotification<UserLevelUpEvent>>
    {
        private IMediator _mediator;

        public async Task Handle(DomainEventNotification<UserLevelUpEvent> notification, CancellationToken cancellationToken)
        {

            await _mediator.Send(new ApplyLevelRolesCommand
            {
                MemberId = notification.DomainEvent.UserId,
                Level = notification.DomainEvent.NewLvl
            });
        }
    }
}