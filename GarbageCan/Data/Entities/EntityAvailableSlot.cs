using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Data.Entities
{
    public class EntityAvailableSlot
    {
        [Key] public int id { get; set; }
        [MaxLength(18)] public string channel_id { get; set; }
    }
}