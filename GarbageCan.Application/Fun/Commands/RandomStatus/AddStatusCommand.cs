using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Presence;
using GarbageCan.Domain.Enums;
using MediatR;

namespace GarbageCan.Application.Fun.Commands.RandomStatus
{
    public class AddStatusCommand : IRequest
    {
        public string Name { get; set; }
        public Activity Activity { get; set; }
    }

    public class AddStatusCommandHandler : IRequestHandler<AddStatusCommand>
    {
        private readonly IApplicationDbContext _context;

        public AddStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AddStatusCommand request, CancellationToken cancellationToken)
        {
            _context.CustomStatuses.Add(new CustomStatus
            {
                Name = request.Name,
                Activity = request.Activity
            });
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
