using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Data.Entities.Roles
{
    public class EntityReactionRole
    {
        [Key] public int id { get; set; }
        public ulong messageId { get; set; }
        public ulong channelId { get; set; }
        public ulong emoteId { get; set; }
        public ulong roleId { get; set; }
    }
}