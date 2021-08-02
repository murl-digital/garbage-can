using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.Commands.RemoveConditionalRole
{
    public class RemoveConditionalRoleCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class RemoveConditionalRoleCommandHandler : IRequestHandler<RemoveConditionalRoleCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordResponseService _responseService;

        public RemoveConditionalRoleCommandHandler(IApplicationDbContext context,
            IDiscordResponseService responseService)
        {
            _context = context;
            _responseService = responseService;
        }

        public async Task<Unit> Handle(RemoveConditionalRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.ConditionalRoles.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (role == null) throw new NotFoundException("Couldn't find Conditional Role");

            _context.ConditionalRoles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);

            await _responseService.RespondAsync("Role removed successfully", true);
            return Unit.Value;
        }
    }
}
