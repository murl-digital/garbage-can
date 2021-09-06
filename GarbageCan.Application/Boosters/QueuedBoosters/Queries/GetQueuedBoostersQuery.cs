using System;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;

namespace GarbageCan.Application.Boosters.QueuedBoosters.Queries
{
    public class GetQueuedBoostersQuery : IRequest<QueuedBooster[]>
    {
        public ulong GuildId { get; set; }
    }

    public class GetQueuedBoostersQueryHandler : RequestHandler<GetQueuedBoostersQuery, QueuedBooster[]>
    {
        private readonly IBoosterService _boosterService;

        public GetQueuedBoostersQueryHandler(IBoosterService boosterService)
        {
            _boosterService = boosterService;
        }

        protected override QueuedBooster[] Handle(GetQueuedBoostersQuery request)
        {
            return _boosterService.QueuedBoosters.TryGetValue(request.GuildId, out var boosters)
                ? boosters.ToArray()
                : Array.Empty<QueuedBooster>();
        }
    }
}
