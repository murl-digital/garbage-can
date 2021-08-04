using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Boosters.Queries
{
    public class GetUserBoostersQuery : IRequest<UserBooster[]>
    {
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
    }

    public class GetUserBoostersQueryHandler : IRequestHandler<GetUserBoostersQuery, UserBooster[]>
    {
        private readonly IApplicationDbContext _context;

        public GetUserBoostersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserBooster[]> Handle(GetUserBoostersQuery request, CancellationToken cancellationToken)
        {
            return await _context.XPUserBoosters
                .Where(b => b.GuildId == request.GuildId)
                .Where(b => b.UserId == request.UserId)
                .ToArrayAsync(cancellationToken);
        }
    }
}
