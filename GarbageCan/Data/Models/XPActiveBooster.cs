using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data
{
    public class XPActiveBooster
    {
        [ForeignKey("id")] [Key] public XPAvailableSlot slot { get; set; } = new XPAvailableSlot();
        [Column(TypeName = "datetime")] public DateTime expiration_date { get; set; }
        public float multipler { get; set; }
    }
}