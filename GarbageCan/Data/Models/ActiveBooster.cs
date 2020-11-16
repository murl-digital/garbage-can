using System;

namespace GarbageCan.Data.Models
{
    public class ActiveBooster
    {
        public AvailableSlot slot { get; set; }
        public DateTime expiration_date { get; set; }
        public float multipler { get; set; }
    }
}