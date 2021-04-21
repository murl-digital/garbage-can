using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.XP;
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
        private readonly IXpCalculatorService _calculator;

        public AddXpToUserCommandHandler(IApplicationDbContext context, IXpCalculatorService calculator)
        {
            _context = context;
            _calculator = calculator;
        }

        public async Task<Unit> Handle(AddXpToUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.XPUsers
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                user = new EntityUser
                {
                    Id = request.UserId,
                    Lvl = 0,
                    XP = 0
                };
                
                _context.XPUsers.Add(user);
            }

            user.XP += _calculator.XpEarned(request.Message);

            await _context.SaveChangesAsync(cancellationToken);

            return new Unit();
        }
    }
}