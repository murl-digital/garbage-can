using System;

namespace GarbageCan.Domain.Entities.Boosters
{
    public class ActiveBooster
    {
        public virtual AvailableSlot slot { get; set; } = new AvailableSlot();
        public DateTime expirationDate { get; set; }
        public float multipler { get; set; }
    }
}