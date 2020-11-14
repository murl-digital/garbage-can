using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Data
{
    public class XPUser
    {
        [Key] [MaxLength(18)] public string id { get; set; }
        public int lvl { get; set; }
        public double xp { get; set; }
    }
}