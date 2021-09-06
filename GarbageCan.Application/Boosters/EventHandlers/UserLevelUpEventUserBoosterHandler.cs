using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Boosters.UserBoosters.Commands;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Events;
using MediatR;

namespace GarbageCan.Application.Boosters.EventHandlers
{
    public class UserLevelUpEventUserBoosterHandler : INotificationHandler<DomainEventNotification<UserLevelUpEvent>>
    {
        private readonly IMediator _mediator;

        public UserLevelUpEventUserBoosterHandler(IMediator mediator)
        {
            _mediator = mediator;
        }


        public async Task Handle(DomainEventNotification<UserLevelUpEvent> notification, CancellationToken cancellationToken)
        {
            var oldLevel = notification.DomainEvent.OldLvl;
            var newLevel = notification.DomainEvent.NewLvl;

            var levelsEarnedThatGrantBoosters =
                Enumerable.Range(oldLevel + 1, Math.Abs(newLevel - oldLevel))
                    .Where(x => x >= 10 && x % 5 == 0)
                    .ToList();

            foreach (var lvl in levelsEarnedThatGrantBoosters)
            {
                await _mediator.Send(new AddUserBoosterCommand
                {
                    GuildId = notification.DomainEvent.MessageDetails.GuildId,
                    UserId = notification.DomainEvent.MessageDetails.UserId,
                    Multiplier = 1.5f,
                    Duration = TimeSpan.FromMinutes(30)
                }, cancellationToken);
            }
        }
    }
}
