using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Roles;
using MediatR;

namespace GarbageCan.Application.Roles.Commands.AddConditionalRole
{
    public class AddConditionalRoleCommand : IRequest
    {
        public ulong GuildId { get; set; }
        public bool Remain { get; set; }
        public ulong RequiredRoleId { get; set; }
        public ulong ResultRoleId { get; set; }
    }

    public class AddConditionalRoleCommandHandler : IRequestHandler<AddConditionalRoleCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordResponseService _responseService;

        public AddConditionalRoleCommandHandler(IApplicationDbContext context, IDiscordResponseService responseService)
        {
            _context = context;
            _responseService = responseService;
        }

        public async Task<Unit> Handle(AddConditionalRoleCommand request, CancellationToken cancellationToken)
        {
            await _context.ConditionalRoles.AddAsync(new ConditionalRole
            {
                GuildId = request.GuildId,
                RequiredRoleId = request.RequiredRoleId,
                ResultRoleId = request.ResultRoleId,
                Remain = request.Remain
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await _responseService.RespondAsync("Role added successfully", true);
            return Unit.Value;
        }
    }
}
