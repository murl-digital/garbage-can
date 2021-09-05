using System.Linq;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;

namespace GarbageCan.Application.Boosters.ActiveBoosters.Queries
{
    public class GetActiveBoostersQuery : IRequest<ActiveBooster[]>
    {
    }

    public class GetActiveBoostersQueryHandler : RequestHandler<GetActiveBoostersQuery, ActiveBooster[]>
    {
        private readonly IBoosterService _boosterService;

        public GetActiveBoostersQueryHandler(IBoosterService boosterService)
        {
            _boosterService = boosterService;
        }

        protected override ActiveBooster[] Handle(GetActiveBoostersQuery request)
        {
            return _boosterService.ActiveBoosters.SelectMany(x => x.Value).ToArray();
        }
    }
}
