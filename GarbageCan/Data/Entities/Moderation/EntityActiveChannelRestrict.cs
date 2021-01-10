using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities.Moderation
{
    [Table("moderation_active_channel_restricts")]
    public class EntityActiveChannelRestrict
    {
        [Key] public int id { get; set; }
        public ulong uId { get; set; }
        public ulong channelId { get; set; }
        [Column(TypeName = "datetime")] public DateTime expirationDate { get; set; }
    }
}