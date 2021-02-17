using System;

namespace GarbageCan.Data.Models.Boosters
{
    public struct ActiveBooster : IBooster
    {
        public float multiplier { get; set; }
        public AvailableSlot slot { get; set; }
        public DateTime expirationDate { get; set; }
    }
}