using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities.Moderation
{
    [Table("moderation_active_mutes")]
    public class EntityActiveChannelRestrict
    {
        public ulong uId { get; set; }
        public ulong channelId { get; set; }
        [Column(TypeName = "datetime")] public DateTime expirationDate { get; set; }
    }
}