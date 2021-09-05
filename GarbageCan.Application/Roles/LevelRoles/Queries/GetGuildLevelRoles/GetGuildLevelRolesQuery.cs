using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Roles;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.LevelRoles.Queries.GetGuildLevelRoles
{
    public class GetGuildLevelRolesQuery : IRequest<List<LevelRole>>
    {
        public ulong GuildId { get; set; }
    }

    public class GetGuildLevelRolesQueryHandler : IRequestHandler<GetGuildLevelRolesQuery, List<LevelRole>>
    {
        private readonly IApplicationDbContext _context;

        public GetGuildLevelRolesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LevelRole>> Handle(GetGuildLevelRolesQuery request, CancellationToken cancellationToken)
        {
            var levelRoles = await _context.LevelRoles
                .Where(r => r.GuildId == request.GuildId)
                .ToListAsync(cancellationToken);
            return levelRoles;
        }
    }
}
