using System;
using System.Linq;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;

namespace GarbageCan.Application.Boosters.QueuedBoosters.Queries
{
    public class GetAllQueuedBoostersQuery : IRequest<QueuedBooster[]>
    {
    }

    public class GetAllQueuedBoostersQueryHandler : RequestHandler<GetAllQueuedBoostersQuery, QueuedBooster[]>
    {
        private readonly IBoosterService _boosterService;

        public GetAllQueuedBoostersQueryHandler(IBoosterService boosterService)
        {
            _boosterService = boosterService;
        }

        protected override QueuedBooster[] Handle(GetAllQueuedBoostersQuery request)
        {
            return _boosterService.QueuedBoosters.SelectMany(x => x.Value).ToArray();
        }
    }
}
