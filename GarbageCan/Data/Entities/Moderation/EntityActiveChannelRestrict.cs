using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data.Entities.Moderation
{
    [Table("moderation_active_channel_restricts")]
    [Keyless]
    public class EntityActiveChannelRestrict
    {
        public ulong uId { get; set; }
        public ulong channelId { get; set; }
        [Column(TypeName = "datetime")] public DateTime expirationDate { get; set; }
    }
}