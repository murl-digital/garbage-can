using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities.Boosters
{
    public class EntityActiveBooster
    {
        [ForeignKey("id")] [Key] public virtual EntityAvailableSlot slot { get; set; } = new();
        [Column(TypeName = "datetime")] public DateTime expirationDate { get; set; }
        public float multipler { get; set; }
    }
}