using System;

namespace GarbageCan.Domain.Entities.Boosters
{
    public class ActiveBooster
    {
        public virtual AvailableSlot Slot { get; set; } = new();
        public ulong GuildId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public float Multiplier { get; set; }
    }
}
