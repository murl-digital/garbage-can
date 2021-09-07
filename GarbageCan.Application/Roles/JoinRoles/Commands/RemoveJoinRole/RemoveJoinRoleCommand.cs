using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.JoinRoles.Commands.RemoveJoinRole
{
    public class RemoveJoinRoleCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class RemoveJoinRoleCommandHandler : IRequestHandler<RemoveJoinRoleCommand>
    {
        private readonly IApplicationDbContext _context;

        public RemoveJoinRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(RemoveJoinRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.JoinRoles.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (role == null) throw new NotFoundException("Couldn't find Join Role");

            _context.JoinRoles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
