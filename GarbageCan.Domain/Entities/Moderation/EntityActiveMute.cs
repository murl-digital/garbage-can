using System;

namespace GarbageCan.Domain.Entities.Moderation
{
    public class EntityActiveMute
    {
        public int id { get; set; }
        public ulong uId { get; init; }
        public DateTime expirationDate { get; init; }
    }
}