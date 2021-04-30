using GarbageCan.Application.Common.Configuration;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Moderation;
using GarbageCan.Domain.Enums;
using Humanizer;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Moderation.Commands.Mute
{
    public class MuteCommand : IRequest<Unit>
    {
        public ulong GuildId { get; set; }
        public string UserDisplayName { get; set; }
        public ulong UserId { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public string Comments { get; set; }
    }

    public class MuteCommandHandler : IRequestHandler<MuteCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly IApplicationDbContext _dbContext;
        private readonly IDiscordDirectMessageService _directMessageService;
        private readonly IDiscordResponseService _responseService;
        private readonly IRoleConfiguration _roleConfiguration;
        private readonly IDiscordGuildRoleService _roleService;

        public MuteCommandHandler(IDiscordGuildRoleService roleService,
            IDiscordResponseService responseService,
            IDateTime dateTime,
            IApplicationDbContext dbContext,
            IRoleConfiguration roleConfiguration,
            IDiscordDirectMessageService directMessageService,
            ICurrentUserService currentUserService)
        {
            _roleService = roleService;
            _responseService = responseService;
            _dateTime = dateTime;
            _dbContext = dbContext;
            _roleConfiguration = roleConfiguration;
            _directMessageService = directMessageService;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(MuteCommand request, CancellationToken cancellationToken)
        {
            await _roleService.GrantRoleAsync(request.GuildId, _roleConfiguration.MuteRoleId, request.UserId, "user muted for " + request.TimeSpan.Humanize());

            await AddActiveMute(request, cancellationToken);
            await AddActionLog(request, cancellationToken);

            var message = $"You have been muted for {request.TimeSpan.Humanize()}.\n\nAdditional comments: {request.Comments}";
            await _directMessageService.SendMessageAsync(request.UserId, message);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _responseService.RespondAsync($"{request.UserDisplayName} has been muted", true);

            return Unit.Value;
        }

        private async Task AddActionLog(MuteCommand request, CancellationToken cancellationToken)
        {
            var log = new ActionLog
            {
                uId = request.UserId,
                mId = ulong.Parse(_currentUserService.UserId),
                issuedDate = _dateTime.Now.ToUniversalTime(),
                punishmentLevel = PunishmentLevel.Mute,
                comments = $"Muted for {request.TimeSpan.Humanize()}. Additional comments: {request.Comments}"
            };

            await _dbContext.ModerationActionLogs.AddAsync(log, cancellationToken);
        }

        private async Task AddActiveMute(MuteCommand request, CancellationToken cancellationToken)
        {
            var mute = new ActiveMute
            {
                uId = request.UserId,
                expirationDate = _dateTime.Now.ToUniversalTime().Add(request.TimeSpan)
            };
            await _dbContext.ModerationActiveMutes.AddAsync(mute, cancellationToken);
        }
    }
}