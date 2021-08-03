using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GarbageCan.Domain.Entities.Boosters;
using GarbageCan.Domain.Enums;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IBoosterService
    {
        public ReadOnlyDictionary<ulong, ReadOnlyCollection<ActiveBooster>> ActiveBoosters { get; }
        public ReadOnlyDictionary<ulong, ReadOnlyCollection<QueuedBooster>> QueuedBoosters { get; }
        public ReadOnlyDictionary<ulong, ReadOnlyCollection<AvailableSlot>> AvailableSlots { get; }

        public float GetMultiplier(ulong guildId);
        public Task<BoosterResult> AddBooster(ulong guildId, float multiplier, TimeSpan duration, bool queue);
        public Task AddSlot(ulong guildId, ulong channelId);
        public Task RemoveSlot(ulong guildId, int id);
    }
}
