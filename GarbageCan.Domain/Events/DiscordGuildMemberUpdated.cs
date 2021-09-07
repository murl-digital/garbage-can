using GarbageCan.Domain.Common;

namespace GarbageCan.Domain.Events
{
    public class DiscordGuildMemberUpdated : DomainEvent
    {
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public bool IsBot { get; set; }
        public bool? IsPending { get; set; }
    }
}