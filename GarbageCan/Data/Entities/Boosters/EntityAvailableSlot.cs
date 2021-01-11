using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities.Boosters
{
    [Table("xpAvailableSlots")]
    public class EntityAvailableSlot
    {
        [Key] public int id { get; set; }
        public ulong channelId { get; set; }
    }
}