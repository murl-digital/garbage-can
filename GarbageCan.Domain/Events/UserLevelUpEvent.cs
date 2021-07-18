using GarbageCan.Domain.Common;

namespace GarbageCan.Domain.Events
{
    public class UserLevelUpEvent : DomainEvent
    {
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public int OldLvl { get; set; }
        public int NewLvl { get; set; }
    }
}