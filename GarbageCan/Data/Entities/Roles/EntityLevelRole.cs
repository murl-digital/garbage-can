using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Data.Entities.Roles
{
    public class EntityLevelRole
    {
        [Key] public int id { get; set; }
        public int lvl { get; set; }
        public ulong roleId { get; set; }
        public bool remain { get; set; }
    }
}