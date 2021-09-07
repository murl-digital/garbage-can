namespace GarbageCan.Domain.Entities.Boosters
{
    public class UserBooster
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public float Multiplier { get; set; }
        public ulong DurationInSeconds { get; set; }
    }
}
