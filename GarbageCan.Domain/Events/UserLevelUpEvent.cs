using GarbageCan.Domain.Common;

namespace GarbageCan.Domain.Events
{
    public class UserLevelUpEvent : DomainEvent
    {
        public MessageDetails MessageDetails { get; set; }
        public int OldLvl { get; set; }
        public int NewLvl { get; set; }
    }
}
