using GarbageCan.Domain.Common;

namespace GarbageCan.Domain.Events
{
    public class DiscordGuildMemberAdded : DomainEvent
    {
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public bool IsBot { get; set; }
    }
}
