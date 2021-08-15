namespace GarbageCan.Domain.Events
{
    public class MessageDetails
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string UserAvatarUrl { get; set; }
        public string Message { get; set; }
    }
}
