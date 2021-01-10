using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities.Boosters
{
    [Table("xp_user_boosters")]
    public class EntityUserBooster
    {
        [Key] public string id { get; set; }
        public ulong userId { get; set; }
        public float multiplier { get; set; }
        public long durationInSeconds { get; set; }
    }
}