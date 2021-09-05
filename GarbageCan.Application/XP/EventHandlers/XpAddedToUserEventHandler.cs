using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.XP.EventHandlers
{
    public class XpAddedToUserEventHandler : INotificationHandler<DomainEventNotification<XpAddedToUserEvent>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDomainEventService _provider;
        private readonly IXpCalculatorService _calculator;

        public XpAddedToUserEventHandler(IApplicationDbContext context, IDomainEventService provider, IXpCalculatorService calculator)
        {
            _context = context;
            _provider = provider;
            _calculator = calculator;
        }

        public async Task Handle(DomainEventNotification<XpAddedToUserEvent> notification, CancellationToken cancellationToken)
        {
            var user = await _context.XPUsers.FirstOrDefaultAsync(u => u.UserId == notification.DomainEvent.MessageDetails.UserId, cancellationToken);

            var oldLevel = user.Lvl;
            while (user.XP > await _calculator.TotalXpRequired(user.Lvl))
            {
                user.Lvl++;
            }

            if (oldLevel < user.Lvl)
            {
                await _context.SaveChangesAsync(cancellationToken);

                await _provider.Publish(new UserLevelUpEvent
                {
                    MessageDetails = notification.DomainEvent.MessageDetails,
                    OldLvl = oldLevel,
                    NewLvl = user.Lvl
                });
            }
        }
    }
}
