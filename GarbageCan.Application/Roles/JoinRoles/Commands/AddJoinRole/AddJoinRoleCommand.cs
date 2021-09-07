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
        public AddJoinRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AddJoinRoleCommand request, CancellationToken cancellationToken)
        {
            await _context.JoinRoles.AddAsync(new JoinRole
            {
                GuildId = request.GuildId,
                RoleId = request.RoleId
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
