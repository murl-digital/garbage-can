using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Roles.Commands.RemoveLevelRole
{
    public class RemoveLevelRoleCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class RemoveLevelRoleCommandHandler : IRequestHandler<RemoveLevelRoleCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordResponseService _responseService;

        public RemoveLevelRoleCommandHandler(IApplicationDbContext context, IDiscordResponseService responseService)
        {
            _context = context;
            _responseService = responseService;
        }

        public async Task<Unit> Handle(RemoveLevelRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.LevelRoles.FirstOrDefaultAsync(x => x.id == request.Id, cancellationToken);
            if (role == null)
            {
                throw new NotFoundException("Couldn't find Level Role");
            }

            _context.LevelRoles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);

            await _responseService.RespondAsync("Role removed successfully", true);
            return Unit.Value;
        }
    }
}