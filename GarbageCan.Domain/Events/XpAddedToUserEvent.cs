using GarbageCan.Domain.Common;

namespace GarbageCan.Domain.Events
{
    public class XpAddedToUserEvent : DomainEvent
    {
        public MessageDetails MessageDetails { get; set; }
        public double OldXp { get; set; }
        public double NewXp { get; set; }
    }
}
