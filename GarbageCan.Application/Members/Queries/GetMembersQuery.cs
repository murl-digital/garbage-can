using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Members.Queries
{
    public class GetMembersQuery : IRequest<List<MemberVm>>
    {
        public ulong? GuildId { get; set; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
    }

    public class GetMembersQueryHandler : IRequestHandler<GetMembersQuery, List<MemberVm>>
    {
        private readonly IDiscordGuildMemberService _service;
        private readonly IApplicationDbContext _context;

        public GetMembersQueryHandler(IDiscordGuildMemberService service, IApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        public async Task<List<MemberVm>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
        {
            var pageSize = request.PageSize;
            var pageNumber = request.PageNumber;

            var result = new List<MemberVm>();
            var members = _service.GetGuildMembers(request.GuildId)
                .Where(m => !m.IsBot)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArray();

            foreach (var member in members)
            {
                var xpMember = await _context.XPUsers.FirstOrDefaultAsync(u => u.UserId == member.Id, cancellationToken);

                result.Add(new MemberVm
                {
                    Id = member.Id,
                    Name = member.DisplayName,
                    Xp = xpMember?.XP ?? 0,
                    Level = xpMember?.Lvl ?? 0,
                    IsBot = member.IsBot
                });
            }

            return result;
        }
    }
}
