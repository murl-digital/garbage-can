using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.LevelRoles.Commands.RemoveLevelRole
{
    public class RemoveLevelRoleCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class RemoveLevelRoleCommandHandler : IRequestHandler<RemoveLevelRoleCommand>
    {
        private readonly IApplicationDbContext _context;

        public RemoveLevelRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(RemoveLevelRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.LevelRoles.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (role == null) throw new NotFoundException("Couldn't find Level Role");

            _context.LevelRoles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
