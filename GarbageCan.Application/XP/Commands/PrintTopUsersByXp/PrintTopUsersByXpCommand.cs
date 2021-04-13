using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.XP.Queries.GetTopUsersByXP;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.XP.Commands.PrintTopUsersByXp
{
    public class PrintTopUsersByXpCommand : IRequest<bool>
    {
        public ulong CurrentUserId { get; set; }
        public int Count { get; set; } = 10;
    }

    public class PrintTopUsersByXpCommandHandler : IRequestHandler<PrintTopUsersByXpCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IDiscordResponseService _responseService;

        public PrintTopUsersByXpCommandHandler(IDiscordResponseService responseService, IMediator mediator)
        {
            _mediator = mediator;
            _responseService = responseService;
        }

        public async Task<bool> Handle(PrintTopUsersByXpCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTopUsersByXPQuery
            {
                CurrentUserId = request.CurrentUserId,
                Count = request.Count
            }, cancellationToken);

            var topTenUsers = result.TopTenUsers;
            topTenUsers.Add(result.ContextUser);

            var lines = topTenUsers.Select(u => $"{u.Place} - {u.DisplayName} (lvl {u.User.Lvl}, {u.User.XP} xp)").ToList();
            lines.Insert(lines.Count - 1, "--");

            await _responseService.RespondAsync(string.Join(Environment.NewLine, lines), formatAsBlock: true);

            return true;
        }
    }
}