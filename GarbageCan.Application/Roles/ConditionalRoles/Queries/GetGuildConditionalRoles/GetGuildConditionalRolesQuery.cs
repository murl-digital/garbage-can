using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Roles;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.ConditionalRoles.Queries.GetGuildConditionalRoles
{
    public class GetGuildConditionalRolesQuery : IRequest<List<ConditionalRole>>
    {
        public ulong GuildId { get; set; }
    }

    public class GetGuildConditionalRolesQueryHandler : IRequestHandler<GetGuildConditionalRolesQuery, List<ConditionalRole>>
    {
        private readonly IApplicationDbContext _context;

        public GetGuildConditionalRolesQueryHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ConditionalRole>> Handle(GetGuildConditionalRolesQuery request, CancellationToken cancellationToken)
        {
            var conditionalRoles = await _context.ConditionalRoles
                .Where(r => r.GuildId == request.GuildId)
                .ToListAsync(cancellationToken);
            return conditionalRoles;
        }
    }
}
