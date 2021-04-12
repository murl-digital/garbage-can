using GarbageCan.Application.Common.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.XP.Queries.GetTopUsersByXP
{
    public class GetTopUsersByXPQuery : IRequest<GetTopUsersByXPQueryVm>
    {
        public ulong CurrentUserId { get; set; }
        public int Count { get; set; } = 10;
    }

    public class GetTopUsersByXPQueryHandler : IRequestHandler<GetTopUsersByXPQuery, GetTopUsersByXPQueryVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordGuildService _discordGuildService;

        public GetTopUsersByXPQueryHandler(IApplicationDbContext context, IDiscordGuildService discordGuildService)
        {
            _context = context;
            _discordGuildService = discordGuildService;
        }

        public async Task<GetTopUsersByXPQueryVm> Handle(GetTopUsersByXPQuery request, CancellationToken cancellationToken)
        {
            var users = _context.XPUsers
                .OrderByDescending(u => u.XP)
                .ToList() // Enumeration needed first as EF Core can't handle the select with an Index
                .Select((u, i) => new GetTopUsersByXPQueryVmUser(u, i))
                .ToList();

            var topUsers = users.Take(request.Count).ToList();

            foreach (var user in topUsers)
            {
                user.DisplayName = await _discordGuildService.GetMemberDisplayNameAsync(user.User.Id);
            }

            var contextUser = users.First(x => x.User.Id == request.CurrentUserId);

            return new GetTopUsersByXPQueryVm
            {
                TopTenUsers = users,
                ContextUser = contextUser
            };
        }
    }
}