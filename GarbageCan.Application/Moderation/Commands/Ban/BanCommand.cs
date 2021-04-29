using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Moderation;
using GarbageCan.Domain.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Moderation.Commands.Ban
{
    public class BanCommand : IRequest
    {
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string Reason { get; set; }
    }

    public class BanCommandHandler : IRequestHandler<BanCommand>
    {
        private readonly IDateTime _dateTime;
        private readonly IApplicationDbContext _dbContext;
        private readonly IDiscordModerationService _moderationService;
        private readonly IDiscordResponseService _responseService;
        private readonly ICurrentUserService _currentUserService;

        public BanCommandHandler(IDiscordModerationService moderationService,
            IDiscordResponseService responseService,
            IDateTime dateTime,
            IApplicationDbContext dbContext,
            ICurrentUserService currentUserService)
        {
            _moderationService = moderationService;
            _responseService = responseService;
            _dateTime = dateTime;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(BanCommand request, CancellationToken cancellationToken)
        {
            await _moderationService.BanAsync(request.GuildId, request.UserId, request.Reason);

            var log = new ActionLog
            {
                uId = request.UserId,
                mId = ulong.Parse(_currentUserService.UserId),
                issuedDate = _dateTime.Now.ToUniversalTime(),
                punishmentLevel = PunishmentLevel.Ban,
                comments = request.Reason
            };

            await _dbContext.ModerationActionLogs.AddAsync(log, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _responseService.RespondAsync($"{request.UserDisplayName} has been banned", true);

            return Unit.Value;
        }
    }
}