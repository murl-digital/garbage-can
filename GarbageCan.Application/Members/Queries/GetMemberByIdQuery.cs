using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Members.Queries
{
    public class GetMemberByIdQuery : IRequest<MemberVm>
    {
        public ulong Id { get; init; }
    }

    public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, MemberVm>
    {
        private readonly IDiscordGuildMemberService _service;
        private readonly IApplicationDbContext _context;

        public GetMemberByIdQueryHandler(IDiscordGuildMemberService service, IApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        public async Task<MemberVm> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
        {
            var member = await _service.GetMemberAsync(request.Id);

            if (member == null)
            {
                return null;
            }

            var xpMember = await _context.XPUsers.FirstOrDefaultAsync(u => u.UserId == member.Id, cancellationToken);

            return new MemberVm
            {
                Id = member.Id,
                Name = member.DisplayName,
                Xp = xpMember?.XP ?? 0,
                Level = xpMember?.Lvl ?? 0,
                IsBot = member.IsBot
            };
        }
    }
}
