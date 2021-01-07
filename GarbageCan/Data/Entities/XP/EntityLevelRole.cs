using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data.Entities.XP
{
    [Table("xp_level_roles")]
    [Keyless]
    public class EntityLevelRole
    {
        public int lvl { get; set; }
        public ulong id { get; set; }
        public bool remain { get; set; }
    }
}