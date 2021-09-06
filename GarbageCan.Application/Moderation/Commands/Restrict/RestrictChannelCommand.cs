using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Moderation;
using GarbageCan.Domain.Enums;
using Humanizer;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Moderation.Commands.Restrict
{
    public class RestrictChannelCommand : IRequest<Unit>
    {
        public ulong ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string Comments { get; set; }
        public ulong GuildId { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public ulong UserId { get; set; }
    }

    public class RestrictChannelCommandHandler : IRequestHandler<RestrictChannelCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly IApplicationDbContext _dbContext;
        private readonly IDiscordDirectMessageService _directMessageService;
        private readonly IDiscordModerationService _moderationService;

        public RestrictChannelCommandHandler(IDiscordModerationService moderationService,
            IDateTime dateTime,
            IApplicationDbContext dbContext,
            IDiscordDirectMessageService directMessageService,
            ICurrentUserService currentUserService)
        {
            _moderationService = moderationService;
            _dateTime = dateTime;
            _dbContext = dbContext;
            _directMessageService = directMessageService;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(RestrictChannelCommand request, CancellationToken cancellationToken)
        {
            await _moderationService.RestrictChannelAccess(request.GuildId, request.UserId, request.ChannelId);

            await AddActiveChannelRestrict(request, cancellationToken);
            await AddActionLog(request, cancellationToken);

            var message = $"Your access to the {request.ChannelName} channel has been restricted for {request.TimeSpan.Humanize()}.\n\nAdditional comments: {request.Comments}";
            await _directMessageService.SendMessageAsync(request.GuildId, request.UserId, message);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        private async Task AddActionLog(RestrictChannelCommand request, CancellationToken cancellationToken)
        {
            var log = new ActionLog
            {
                uId = request.UserId,
                mId = ulong.Parse(_currentUserService.UserId),
                issuedDate = _dateTime.Now.ToUniversalTime(),
                punishmentLevel = PunishmentLevel.ChannelRestrict,
                comments = $"Restricted access to {request.ChannelName} for {request.TimeSpan.Humanize()}. Additional comments: {request.Comments}"
            };

            await _dbContext.ModerationActionLogs.AddAsync(log, cancellationToken);
        }

        private async Task AddActiveChannelRestrict(RestrictChannelCommand request, CancellationToken cancellationToken)
        {
            var restrict = new ActiveChannelRestrict
            {
                uId = request.UserId,
                channelId = request.ChannelId,
                expirationDate = _dateTime.Now.ToUniversalTime().Add(request.TimeSpan)
            };
            await _dbContext.ModerationActiveChannelRestricts.AddAsync(restrict, cancellationToken);
        }
    }
}
