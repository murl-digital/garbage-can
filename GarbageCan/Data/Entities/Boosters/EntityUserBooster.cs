using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities.Boosters
{
    [Table("xpUserBoosters")]
    public class EntityUserBooster
    {
        [Key] public int id { get; set; }
        public ulong userId { get; set; }
        public float multiplier { get; set; }
        public long durationInSeconds { get; set; }
    }
}