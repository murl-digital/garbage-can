using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Roles.Commands.RemoveJoinRole
{
    public class RemoveJoinRoleCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class RemoveJoinRoleCommandHandler : IRequestHandler<RemoveJoinRoleCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordResponseService _responseService;

        public RemoveJoinRoleCommandHandler(IApplicationDbContext context, IDiscordResponseService responseService)
        {
            _context = context;
            _responseService = responseService;
        }

        public async Task<Unit> Handle(RemoveJoinRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.JoinRoles.FirstOrDefaultAsync(x => x.id == request.Id, cancellationToken);
            if (role == null)
            {
                throw new NotFoundException("Couldn't find Join Role");
            }

            _context.JoinRoles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);

            await _responseService.RespondAsync("Role removed successfully", true);
            return Unit.Value;
        }
    }
}