using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data.Entities.XP
{
    [Table("xp_excluded_channels")]
    [Keyless]
    public class EntityExcludedChannel
    {
        public ulong channel_id { get; set; }   
    }
}