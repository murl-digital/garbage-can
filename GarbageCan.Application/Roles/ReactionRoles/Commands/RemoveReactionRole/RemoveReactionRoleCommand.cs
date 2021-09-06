using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.ReactionRoles.Commands.RemoveReactionRole
{
    public class RemoveReactionRoleCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class RemoveReactionRoleCommandHandler : IRequestHandler<RemoveReactionRoleCommand>
    {
        private readonly IApplicationDbContext _context;

        public RemoveReactionRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(RemoveReactionRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.ReactionRoles.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (role == null) throw new NotFoundException("Couldn't find Reaction Role");

            _context.ReactionRoles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}
