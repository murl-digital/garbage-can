using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Roles.Commands.RemoveReactionRole
{
    public class RemoveReactionRoleCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class RemoveReactionRoleCommandHandler : IRequestHandler<RemoveReactionRoleCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordResponseService _responseService;

        public RemoveReactionRoleCommandHandler(IApplicationDbContext context, IDiscordResponseService responseService)
        {
            _context = context;
            _responseService = responseService;
        }

        public async Task<Unit> Handle(RemoveReactionRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.ReactionRoles.FirstOrDefaultAsync(x => x.id == request.Id, cancellationToken);
            if (role == null)
            {
                throw new NotFoundException("Couldn't find Reaction Role");
            }

            _context.ReactionRoles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);

            await _responseService.RespondAsync("Role removed successfully", true);
            return Unit.Value;
        }
    }
}