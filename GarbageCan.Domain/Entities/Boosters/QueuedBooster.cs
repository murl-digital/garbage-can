namespace GarbageCan.Domain.Entities.Boosters
{
    public class QueuedBooster
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public uint Position { get; set; }
        public float Multiplier { get; set; }
        public long DurationInSeconds { get; set; }
    }
}
