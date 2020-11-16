using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Data.Entities
{
    public class EntityUserBooster
    {
        [Key] public string id { get; set; }
        [MaxLength(18)] public string user_id { get; set; }
        public float multiplier { get; set; }
        public long duration_in_seconds { get; set; }
    }
}