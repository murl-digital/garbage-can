using GarbageCan.Domain.Common;

namespace GarbageCan.Domain.Events
{
    public class DiscordGuildMemberAdded : DomainEvent
    {
        public ulong UserId { get; set; }
        public bool IsBot { get; set; }
    }
}