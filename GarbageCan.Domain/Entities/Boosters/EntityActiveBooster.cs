using System;

namespace GarbageCan.Domain.Entities.Boosters
{
    public class EntityActiveBooster
    {
        public virtual EntityAvailableSlot slot { get; set; } = new EntityAvailableSlot();
        public DateTime expirationDate { get; set; }
        public float multipler { get; set; }
    }
}