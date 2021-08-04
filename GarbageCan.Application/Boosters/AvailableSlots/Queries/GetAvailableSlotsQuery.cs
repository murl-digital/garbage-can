using System;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;

namespace GarbageCan.Application.Boosters.AvailableSlots.Queries
{
    public class GetAvailableSlotsQuery : IRequest<AvailableSlot[]>
    {
        public ulong GuildId { get; set; }
    }

    public class GetQueuedBoostersQueryHandler : RequestHandler<GetAvailableSlotsQuery, AvailableSlot[]>
    {
        private readonly IBoosterService _boosterService;

        public GetQueuedBoostersQueryHandler(IBoosterService boosterService)
        {
            _boosterService = boosterService;
        }

        protected override AvailableSlot[] Handle(GetAvailableSlotsQuery request)
        {
            return _boosterService.AvailableSlots.TryGetValue(request.GuildId, out var boosters)
                ? boosters.ToArray()
                : Array.Empty<AvailableSlot>();
        }
    }
}
