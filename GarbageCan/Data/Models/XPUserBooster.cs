using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data
{
    public class XPUserBooster
    {
        [Key] public string id { get; set; }
        [MaxLength(18)] public string user_id { get; set; }
        public float multiplier { get; set; }
        public long duration_in_seconds { get; set; }
    }
}