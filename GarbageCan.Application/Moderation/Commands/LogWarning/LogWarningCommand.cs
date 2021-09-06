using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Moderation;
using GarbageCan.Domain.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Moderation.Commands.LogWarning
{
    public class LogWarningCommand : IRequest<int>
    {
        public ulong UserId { get; set; }
        public string Comments { get; set; }
    }

    public class LogWarningCommandHandler : IRequestHandler<LogWarningCommand, int>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _context;
        private readonly IDateTime _dateTime;

        public LogWarningCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext context, IDateTime dateTime)
        {
            _currentUserService = currentUserService;
            _context = context;
            _dateTime = dateTime;
        }

        public async Task<int> Handle(LogWarningCommand request, CancellationToken cancellationToken)
        {
            var now = _dateTime.Now.ToUniversalTime();
            var tryParse = ulong.TryParse(_currentUserService.UserId, out ulong test);
            var log = new ActionLog
            {
                uId = request.UserId,
                mId = tryParse ? test : 0,
                issuedDate = now,
                punishmentLevel = PunishmentLevel.VerbalWarning,
                comments = request.Comments
            };
            await _context.ModerationActionLogs.AddAsync(log, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return log.id;
        }
    }
}
