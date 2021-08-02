using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Models;
using GarbageCan.Application.Roles.Commands.ApplyLevelRoles;
using GarbageCan.Domain.Events;
using MediatR;

namespace GarbageCan.Application.Roles.EventHandlers
{
    public class UserLevelUpEventHandler : INotificationHandler<DomainEventNotification<UserLevelUpEvent>>
    {
        private readonly IMediator _mediator;

        public UserLevelUpEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(DomainEventNotification<UserLevelUpEvent> notification,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new ApplyLevelRolesCommand
            {
                GuildId = notification.DomainEvent.GuildId,
                MemberId = notification.DomainEvent.UserId,
                Level = notification.DomainEvent.NewLvl
            });
        }
    }
}
