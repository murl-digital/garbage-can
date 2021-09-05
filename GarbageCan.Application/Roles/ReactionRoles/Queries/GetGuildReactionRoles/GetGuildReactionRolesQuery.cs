using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Roles;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.ReactionRoles.Queries.GetGuildReactionRoles
{
    public class GetGuildReactionRolesQuery : IRequest<List<ReactionRole>>
    {
        public ulong GuildId { get; set; }
    }

    public class GetGuildLevelRolesQueryHandler : IRequestHandler<GetGuildReactionRolesQuery, List<ReactionRole>>
    {
        private readonly IApplicationDbContext _context;

        public GetGuildLevelRolesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReactionRole>> Handle(GetGuildReactionRolesQuery request, CancellationToken cancellationToken)
        {
            var reactionRoles = await _context.ReactionRoles
                .Where(r => r.GuildId == request.GuildId)
                .ToListAsync(cancellationToken);

            return reactionRoles;
        }
    }
}
