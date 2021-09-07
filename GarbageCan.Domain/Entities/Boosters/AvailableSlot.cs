namespace GarbageCan.Domain.Entities.Boosters
{
    public class AvailableSlot
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
    }
}
