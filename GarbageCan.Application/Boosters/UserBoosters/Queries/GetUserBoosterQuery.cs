using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;

namespace GarbageCan.Application.Boosters.UserBoosters.Queries
{
    public class GetUserBoosterQuery : IRequest<UserBooster>
    {
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public int Id { get; set; }
    }

    public class GetUserBoosterQueryHandler : IRequestHandler<GetUserBoosterQuery, UserBooster>
    {
        private readonly IMediator _mediator;

        public GetUserBoosterQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<UserBooster> Handle(GetUserBoosterQuery request, CancellationToken cancellationToken)
        {
            var boosters = await _mediator.Send(new GetUserBoostersQuery
            {
                GuildId = request.GuildId,
                UserId = request.UserId
            });

            return boosters.FirstOrDefault(b => b.Id == request.Id);
        }
    }
}
