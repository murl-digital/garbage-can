using System;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;

namespace GarbageCan.Application.Boosters.ActiveBoosters.Queries
{
    public class GetActiveBoostersQuery : IRequest<ActiveBooster[]>
    {
        public ulong GuildId { get; set; }
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
            return _boosterService.ActiveBoosters.TryGetValue(request.GuildId, out var boosters)
                ? boosters.ToArray()
                : Array.Empty<ActiveBooster>();
        }
    }
}
