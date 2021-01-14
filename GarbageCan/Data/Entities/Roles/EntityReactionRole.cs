using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities.Roles
{
    [Table("reactionRoles")]
    public class EntityReactionRole
    {
        [Key] public int id { get; set; }
        public ulong messageId { get; set; }
        public ulong channelId { get; set; }
        public string emoteId { get; set; }
        public ulong roleId { get; set; }
    }
}