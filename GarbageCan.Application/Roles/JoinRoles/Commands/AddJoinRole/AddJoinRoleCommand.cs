using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Roles;
using MediatR;

namespace GarbageCan.Application.Roles.JoinRoles.Commands.AddJoinRole
{
    public class AddJoinRoleCommand : IRequest
    {
        public ulong GuildId { get; set; }
        public ulong RoleId { get; set; }
    }

    public class AddJoinRoleCommandHandler : IRequestHandler<AddJoinRoleCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordResponseService _responseService;

        public AddJoinRoleCommandHandler(IApplicationDbContext context, IDiscordResponseService responseService)
        {
            _context = context;
            _responseService = responseService;
        }

        public async Task<Unit> Handle(AddJoinRoleCommand request, CancellationToken cancellationToken)
        {
            await _context.JoinRoles.AddAsync(new JoinRole
            {
                GuildId = request.GuildId,
                RoleId = request.RoleId
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await _responseService.RespondAsync("Role added successfully", true);
            return Unit.Value;
        }
    }
}
