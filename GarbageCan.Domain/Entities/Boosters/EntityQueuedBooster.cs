using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Domain.Entities.Boosters
{
    public class EntityQueuedBooster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int position { get; set; }

        public float multiplier { get; set; }
        public long durationInSeconds { get; set; }
    }
}