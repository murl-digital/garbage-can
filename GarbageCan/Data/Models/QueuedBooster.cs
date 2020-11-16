namespace GarbageCan.Data.Models
{
    public class QueuedBooster
    {
        public int position { get; set; }
        public float multiplier { get; set; }
        public long duration_in_seconds { get; set; }
    }
}