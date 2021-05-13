using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly IDiscordGuildService _discordGuildService;
        private readonly ILogger<GetTopUsersByXPQueryHandler> _logger;

        public GetTopUsersByXPQueryHandler(IApplicationDbContext context,
            IDiscordGuildService discordGuildService,
            ILogger<GetTopUsersByXPQueryHandler> logger)
        {
            _context = context;
            _discordGuildService = discordGuildService;
            _logger = logger;
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
                user.DisplayName = await GetMemberDisplayNameAsync(user.User.Id);
            }

            var contextUser = users.First(x => x.User.Id == request.CurrentUserId);

            return new GetTopUsersByXPQueryVm
            {
                TopTenUsers = topUsers,
                ContextUser = contextUser
            };
        }

        private async Task<string> GetMemberDisplayNameAsync(ulong userId)
        {
            try
            {
                return await _discordGuildService.GetMemberDisplayNameAsync(userId);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Failed to get display name for user: {userId}", userId);
                return userId.ToString();
            }
        }
    }
}