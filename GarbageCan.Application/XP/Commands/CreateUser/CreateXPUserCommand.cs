using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.XP;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.XP.Commands.CreateUser
{
    public class CreateXPUserCommand : IRequest
    {
        public ulong UserId { get; init; }
        public bool IsBot { get; init; }
    }

    public class CreateXPUserCommandHandler : IRequestHandler<CreateXPUserCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateXPUserCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CreateXPUserCommand request, CancellationToken cancellationToken)
        {
            if (request.IsBot)
            {
                return Unit.Value;
            }

            if (await _context.XPUsers.AnyAsync(x => x.UserId == request.UserId, cancellationToken))
            {
                return Unit.Value;
            }

            await _context.XPUsers.AddAsync(new User
            {
                UserId = request.UserId,
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}