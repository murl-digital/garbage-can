using GarbageCan.Domain.Common;

namespace GarbageCan.Domain.Events
{
    public class XpAddedToUserEvent : DomainEvent
    {
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public double OldXp { get; set; }
        public double NewXp { get; set; }
    }
}