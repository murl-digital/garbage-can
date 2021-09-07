using GarbageCan.Domain.Common;
using GarbageCan.Domain.Entities;

namespace GarbageCan.Domain.Events
{
    public class DiscordMessageReactionAddedEvent : DomainEvent
    {
        public ulong ChannelId { get; set; }
        public Emoji Emoji { get; set; }
        public ulong GuildId { get; set; }
        public ulong MessageId { get; set; }
        public ulong UserId { get; set; }
    }
}