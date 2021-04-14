namespace GarbageCan.Domain.Entities.Roles
{
    public class EntityReactionRole
    {
        public int id { get; set; }
        public ulong messageId { get; set; }
        public ulong channelId { get; set; }
        public string emoteId { get; set; }
        public ulong roleId { get; set; }
    }
}