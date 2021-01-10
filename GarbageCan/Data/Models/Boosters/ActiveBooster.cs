using System;

namespace GarbageCan.Data.Models.Boosters
{
    public class ActiveBooster : Booster
    {
        public AvailableSlot slot { get; set; }
        public DateTime expirationDate { get; set; }
    }
}