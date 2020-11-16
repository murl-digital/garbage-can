using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Data.Entities
{
    public class EntityUser
    {
        [Key] [MaxLength(18)] public string id { get; set; }
        public int lvl { get; set; }
        public double xp { get; set; }
    }
}