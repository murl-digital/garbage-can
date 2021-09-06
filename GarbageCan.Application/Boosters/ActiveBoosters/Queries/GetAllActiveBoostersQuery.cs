using System.Linq;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;

namespace GarbageCan.Application.Boosters.ActiveBoosters.Queries
{
    public class GetAllActiveBoostersQuery : IRequest<ActiveBooster[]>
    {
    }

    public class GetAllActiveBoostersQueryHandler : RequestHandler<GetAllActiveBoostersQuery, ActiveBooster[]>
    {
        private readonly IBoosterService _boosterService;

        public GetAllActiveBoostersQueryHandler(IBoosterService boosterService)
        {
            _boosterService = boosterService;
        }

        protected override ActiveBooster[] Handle(GetAllActiveBoostersQuery request)
        {
            return _boosterService.ActiveBoosters.SelectMany(x => x.Value).ToArray();
        }
    }
}
