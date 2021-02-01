using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Data.Entities.Boosters
{
    public class EntityAvailableSlot
    {
        [Key] public int id { get; set; }
        public ulong channelId { get; set; }
    }
}