using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Data.Entities.Config
{
    public class EntityConfig
    {
        [Key] public string key { get; set; }
        public string value { get; set; }
    }
}