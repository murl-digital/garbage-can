using GarbageCan.Application.Common.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.XP.Queries.GetTopUsersByXP
{
    public class GetTopUsersByXPQuery : IRequest<GetTopUsersByXPQueryVm>
    {
        public int Count { get; set; } = 10;
        public ulong CurrentUserId { get; set; }
    }

    public class GetTopUsersByXPQueryHandler : IRequestHandler<GetTopUsersByXPQuery, GetTopUsersByXPQueryVm>
    {
        private readonly IApplicationDbContext _context;

        public GetTopUsersByXPQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetTopUsersByXPQueryVm> Handle(GetTopUsersByXPQuery request, CancellationToken cancellationToken)
        {
            var users = _context.XPUsers
                .OrderByDescending(u => u.XP)
                .ToList() // Enumeration needed first as EF Core can't handle the select with an Index
                .Select((u, i) => new GetTopUsersByXPQueryVmUser(u, i))
                .ToList();

            var topUsers = users.Take(request.Count).ToList();

            var contextUser = users.First(x => x.User.UserId == request.CurrentUserId);

            return new GetTopUsersByXPQueryVm
            {
                TopTenUsers = topUsers,
                ContextUser = contextUser
            };
        }
    }
}
