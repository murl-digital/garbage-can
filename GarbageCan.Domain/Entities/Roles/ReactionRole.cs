namespace GarbageCan.Domain.Entities.Roles
{
    public class ReactionRole
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public string EmoteId { get; set; }
        public ulong RoleId { get; set; }
    }
}
