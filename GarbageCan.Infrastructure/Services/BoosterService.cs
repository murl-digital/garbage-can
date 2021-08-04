using System.Collections.Generic;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;

namespace GarbageCan.Infrastructure.Services
{
    public class BoosterService : IBoosterService
    {
        public Dictionary<ulong, List<ActiveBooster>> ActiveBoosters { get; set; }
        public Dictionary<ulong, Queue<QueuedBooster>> QueuedBoosters { get; set; }
        public Dictionary<ulong, List<AvailableSlot>> AvailableSlots { get; set; }
    }
}
