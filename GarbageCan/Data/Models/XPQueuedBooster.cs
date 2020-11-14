using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Data
{
    public class XPQueuedBooster
    {
        [Key] public int position { get; set; }
        public float multiplier { get; set; }
        public long duration_in_seconds { get; set; }
    }
}