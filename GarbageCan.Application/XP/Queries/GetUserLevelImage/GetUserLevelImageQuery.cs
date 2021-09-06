using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.XP.Queries.GetXPImageStream;
using GarbageCan.Domain.Entities.XP;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.XP.Queries.GetUserLevelImage
{
    public class GetUserLevelImageQuery : IRequest<Stream>
    {
        public string AvatarUrl { get; set; }
        public string MemberDiscriminator { get; set; }
        public ulong MessageId { get; set; }
        public ulong UserId { get; set; }
        public string UserDisplayName { get; set; }
    }

    public class GetUserLevelImageQueryHandler : IRequestHandler<GetUserLevelImageQuery,Stream>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly IXpCalculatorService _xpCalculator;

        public GetUserLevelImageQueryHandler(IApplicationDbContext context,
            IXpCalculatorService xpCalculator,
            IMediator mediator)
        {
            _context = context;
            _xpCalculator = xpCalculator;
            _mediator = mediator;
        }

        public async Task<Stream> Handle(GetUserLevelImageQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.XPUsers.OrderByDescending(x => x.XP)
                .Select(u => u.UserId)
                .ToListAsync(cancellationToken);

            var user = await _context.XPUsers.OrderByDescending(x => x.XP)
                           .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken) ??
                       new User();

            var placement = users.FindIndex(u => u == request.UserId) + 1;

            var currentXp = user.XP - _xpCalculator.TotalXpRequired(user.Lvl - 1);
            var required = _xpCalculator.TotalXpRequired(user.Lvl);
            var progress = currentXp / _xpCalculator.XpRequired(user.Lvl);

            var image = await _mediator.Send(new GetXPImageStreamQuery
            {
                DisplayName = request.UserDisplayName,
                Placement = placement,
                Level = user.Lvl,
                Xp = user.XP,
                Discriminator = request.MemberDiscriminator,
                AvatarUrl = request.AvatarUrl,
                Progress = progress,
                Required = required
            }, cancellationToken);

            return image;
        }
    }
}
