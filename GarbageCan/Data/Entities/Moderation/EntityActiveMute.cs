using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data.Entities.Moderation
{
    [Table("moderation_active_mutes")]
    [Keyless]
    public class EntityActiveMute
    {
        public ulong uId { get; set; }
        [Column(TypeName = "datetime")] public DateTime expirationDate { get; set; }
    }
}