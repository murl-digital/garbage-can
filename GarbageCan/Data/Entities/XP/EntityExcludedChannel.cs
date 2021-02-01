using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data.Entities.XP
{
    [Keyless]
    public class EntityExcludedChannel
    {
        public ulong channelId { get; set; }
    }
}