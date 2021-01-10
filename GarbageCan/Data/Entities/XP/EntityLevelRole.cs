using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data.Entities.XP
{
    [Table("xp_level_roles")]
    public class EntityLevelRole
    {
        [Key] public int id { get; set; }
        public int lvl { get; set; }
        public ulong roleId { get; set; }
        public bool remain { get; set; }
    }
}