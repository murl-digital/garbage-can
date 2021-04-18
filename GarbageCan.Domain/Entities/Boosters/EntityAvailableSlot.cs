using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Domain.Entities.Boosters
{
    public class EntityAvailableSlot
    {
        [Key] public int id { get; set; }
        public ulong channelId { get; set; }
    }
}