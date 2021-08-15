using GarbageCan.Domain.Common;

namespace GarbageCan.Domain.Events
{
    public class DiscordMessageCreatedEvent : DomainEvent
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public ulong AuthorId { get; set; }
        public string AuthorDisplayName { get; set; }
        public string AuthorAvatarUrl { get; set; }
        public string Content { get; set; }
        public bool AuthorIsBot { get; set; }
        public bool AuthorIsSystem { get; set; }
        public bool ChannelIsPrivate { get; set; }
    }
}