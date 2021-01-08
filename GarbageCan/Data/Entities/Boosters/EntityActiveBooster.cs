using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities.Boosters
{
    [Table("xp_active_boosters")]
    public class EntityActiveBooster
    {
        [ForeignKey("id")] [Key] public virtual EntityAvailableSlot slot { get; set; } = new();
        [Column(TypeName = "datetime")] public DateTime expiration_date { get; set; }
        public float multipler { get; set; }
    }
}