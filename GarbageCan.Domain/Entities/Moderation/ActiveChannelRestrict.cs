using System;

namespace GarbageCan.Domain.Entities.Moderation
{
    public class ActiveChannelRestrict
    {
        public int id { get; set; }
        public ulong uId { get; init; }
        public ulong channelId { get; init; }
        public DateTime expirationDate { get; init; }
    }
}