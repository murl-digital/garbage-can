using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Roles;
using MediatR;

namespace GarbageCan.Application.Roles.Commands.AddLevelRole
{
    public class AddLevelRoleCommand : IRequest
    {
        public int Level { get; set; }
        public bool Remain { get; set; }
        public ulong RoleId { get; set; }
        public ulong GuildId { get; set; }
    }

    public class AddLevelRoleCommandHandler : IRequestHandler<AddLevelRoleCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordResponseService _responseService;

        public AddLevelRoleCommandHandler(IApplicationDbContext context, IDiscordResponseService responseService)
        {
            _context = context;
            _responseService = responseService;
        }

        public async Task<Unit> Handle(AddLevelRoleCommand request, CancellationToken cancellationToken)
        {
            await _context.LevelRoles.AddAsync(new LevelRole
            {
                GuildId = request.GuildId,
                RoleId = request.RoleId,
                Lvl = request.Level,
                Remain = request.Remain
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await _responseService.RespondAsync("Role added successfully", true);
            return Unit.Value;
        }
    }
}
