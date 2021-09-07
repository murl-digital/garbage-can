using System.Collections.Generic;
using GarbageCan.Domain.Entities.Boosters;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IBoosterService
    {
        public Dictionary<ulong, List<ActiveBooster>> ActiveBoosters { get; set; }
        public Dictionary<ulong, Queue<QueuedBooster>> QueuedBoosters { get; set; }
        public Dictionary<ulong, List<AvailableSlot>> AvailableSlots { get; set; }
    }
}
