using GarbageCan.Domain.Common;

namespace GarbageCan.Domain.Events
{
    public class DiscordMessageCreatedEvent : DomainEvent
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public ulong AuthorId { get; set; }
        public string Content { get; set; }
    }
}