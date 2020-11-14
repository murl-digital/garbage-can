using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data
{
    public class XPActiveBooster
    {
        public XPAvailableSlot slot { get; set; }
        [Key] [ForeignKey("XPAvailableSlot")] public int slot_id { get; set; }
        [Column(TypeName = "datetime")] public DateTime expiration_date { get; set; }
        public float multipler { get; set; }
    }
}