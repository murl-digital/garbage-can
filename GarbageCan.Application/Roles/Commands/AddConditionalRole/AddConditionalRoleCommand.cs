using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Roles;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Roles.Commands.AddConditionalRole
{
    public class AddConditionalRoleCommand : IRequest
    {
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
                requiredRoleId = request.RequiredRoleId,
                resultRoleId = request.ResultRoleId,
                remain = request.Remain,
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await _responseService.RespondAsync("Role added successfully", true);
            return Unit.Value;
        }
    }
}