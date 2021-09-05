using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Roles;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.JoinRoles.Queries.GetGuildJoinRoles
{
    public class GetGuildJoinRolesQuery : IRequest<List<JoinRole>>
    {
        public ulong GuildId { get; set; }
    }

    public class GetGuildLevelRolesQueryHandler : IRequestHandler<GetGuildJoinRolesQuery, List<JoinRole>>
    {
        private readonly IApplicationDbContext _context;

        public GetGuildLevelRolesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<JoinRole>> Handle(GetGuildJoinRolesQuery request, CancellationToken cancellationToken)
        {
            var joinRoles = await _context.JoinRoles
                .Where(r => r.GuildId == request.GuildId)
                .ToListAsync(cancellationToken);
            
            return joinRoles;
        }
    }
}
