namespace GarbageCan.Domain.Entities.Boosters
{
    public class QueuedBooster
    {
        public int position { get; set; }

        public float multiplier { get; set; }
        public long durationInSeconds { get; set; }
    }
}