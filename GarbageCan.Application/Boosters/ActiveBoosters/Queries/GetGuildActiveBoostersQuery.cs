using System;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;

namespace GarbageCan.Application.Boosters.ActiveBoosters.Queries
{
    public class GetGuildActiveBoostersQuery : IRequest<ActiveBooster[]>
    {
        public ulong GuildId { get; set; }
    }

    public class GetGuildActiveBoostersQueryHandler : RequestHandler<GetGuildActiveBoostersQuery, ActiveBooster[]>
    {
        private readonly IBoosterService _boosterService;

        public GetGuildActiveBoostersQueryHandler(IBoosterService boosterService)
        {
            _boosterService = boosterService;
        }

        protected override ActiveBooster[] Handle(GetGuildActiveBoostersQuery request)
        {
            return _boosterService.ActiveBoosters.TryGetValue(request.GuildId, out var boosters)
                ? boosters.ToArray()
                : Array.Empty<ActiveBooster>();
        }
    }
}
