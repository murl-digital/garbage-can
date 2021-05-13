using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.XP.Queries.GetXPImageStream;
using GarbageCan.Domain.Entities.XP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.XP.Commands.PrintUserLevel
{
    public class PrintUserLevelCommand : IRequest
    {
        public string AvatarUrl { get; set; }
        public string MemberDiscriminator { get; set; }
        public ulong MessageId { get; set; }
        public ulong UserId { get; set; }
    }

    public class PrintUserLevelCommandHandler : IRequestHandler<PrintUserLevelCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordGuildService _guildService;
        private readonly IMediator _mediator;
        private readonly IDiscordResponseService _responseService;
        private readonly IXpCalculatorService _xpCalculator;

        public PrintUserLevelCommandHandler(IApplicationDbContext context,
            IDiscordGuildService guildService,
            IDiscordResponseService responseService,
            IXpCalculatorService xpCalculator,
            IMediator mediator)
        {
            _context = context;
            _guildService = guildService;
            _responseService = responseService;
            _xpCalculator = xpCalculator;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(PrintUserLevelCommand request, CancellationToken cancellationToken)
        {
            var users = await _context.XPUsers.OrderByDescending(x => x.XP)
                .Select(u => u.Id)
                .ToListAsync(cancellationToken);

            var user = await _context.XPUsers.OrderByDescending(x => x.XP)
                           .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken) ??
                       new User();

            var placement = users.FindIndex(u => u == request.UserId) + 1;
            var displayName = await _guildService.GetMemberDisplayNameAsync(request.UserId);

            var currentXp = user.XP - _xpCalculator.TotalXpRequired(user.Lvl - 1);
            var required = _xpCalculator.TotalXpRequired(user.Lvl);
            var progress = currentXp / _xpCalculator.XpRequired(user.Lvl);

            var image = await _mediator.Send(new GetXPImageStreamQuery
            {
                DisplayName = displayName,
                Placement = placement,
                Level = user.Lvl,
                Xp = user.XP,
                Discriminator = request.MemberDiscriminator,
                AvatarUrl = request.AvatarUrl,
                Progress = progress,
                Required = required
            }, cancellationToken);

            await _responseService.RespondWithFileAsync("rank.png", image, request.MessageId, true);

            return Unit.Value;
        }
    }
}