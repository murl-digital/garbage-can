using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Moderation;
using GarbageCan.Domain.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Moderation.Commands.LogTalk
{
    public class LogTalkCommand : IRequest<int>
    {
        public ulong UserId { get; set; }
        public string Comments { get; set; }
    }

    public class LogTalkCommandHandler : IRequestHandler<LogTalkCommand, int>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _context;
        private readonly IDateTime _dateTime;

        public LogTalkCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext context, IDateTime dateTime)
        {
            _currentUserService = currentUserService;
            _context = context;
            _dateTime = dateTime;
        }

        public async Task<int> Handle(LogTalkCommand request, CancellationToken cancellationToken)
        {
            var now = _dateTime.Now.ToUniversalTime();
            var tryParse = ulong.TryParse(_currentUserService.UserId, out ulong test);
            var log = new ActionLog
            {
                uId = request.UserId,
                mId = tryParse ? test : 0,
                issuedDate = now,
                punishmentLevel = PunishmentLevel.PersonalTalk,
                comments = request.Comments
            };
            await _context.ModerationActionLogs.AddAsync(log, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return log.id;
        }
    }
}
