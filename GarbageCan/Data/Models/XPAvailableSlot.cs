using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Data
{
    public class XPAvailableSlot
    {
        [Key] public int id { get; set; }
        [MaxLength(18)] public string channel_id { get; set; }
    }
}