using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data.Entities.XP
{
    [Table("xpExcludedChannels")]
    [Keyless]
    public class EntityExcludedChannel
    {
        public ulong channelId { get; set; }
    }
}