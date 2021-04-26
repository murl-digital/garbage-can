using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.XP;
using GarbageCan.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.XP.Commands.AddXpToUser
{
    public class AddXpToUserCommand : IRequest
    {
        public ulong UserId { get; set; }
        public string Message { get; set; }
    }

    public class AddXpToUserCommandHandler : IRequestHandler<AddXpToUserCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDomainEventService _provider;
        private readonly IXpCalculatorService _calculator;

        public AddXpToUserCommandHandler(IApplicationDbContext context, IDomainEventService provider, IXpCalculatorService calculator)
        {
            _context = context;
            _calculator = calculator;
            _provider = provider;
        }

        public async Task<Unit> Handle(AddXpToUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.XPUsers
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                user = new User
                {
                    Id = request.UserId,
                    Lvl = 0,
                    XP = 0
                };
                
                _context.XPUsers.Add(user);
            }

            var oldXP = user.XP;
            user.XP += _calculator.XpEarned(request.Message);

            await _context.SaveChangesAsync(cancellationToken);

            await _provider.Publish(new XpAddedToUserEvent
            {
                UserId = user.Id,
                OldXp = oldXP,
                NewXp = user.XP
            });

            return new Unit();
        }
    }
}