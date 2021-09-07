using GarbageCan.Domain.Common;

namespace GarbageCan.Domain.Events
{
    public class DiscordGuildUpdatedEvent : DomainEvent
    {
        public ulong GuildId { get; set; }
        public int PreviousBoostCount { get; set; }
        public int BoostCount { get; set; }
    }
}
