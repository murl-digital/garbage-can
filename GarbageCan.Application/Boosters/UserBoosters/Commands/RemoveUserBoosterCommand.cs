using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Z.EntityFramework.Plus;

namespace GarbageCan.Application.Boosters.UserBoosters.Commands
{
    public class RemoveUserBoosterCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class RemoveUserBoosterCommandHandler : IRequestHandler<RemoveUserBoosterCommand>
    {
        private readonly IApplicationDbContext _context;

        public RemoveUserBoosterCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(RemoveUserBoosterCommand request, CancellationToken cancellationToken)
        {
            await _context.XPUserBoosters
                .Where(b => b.Id == request.Id)
                .DeleteAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
