using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data
{
    public class XPContext : DbContext
    {
        public DbSet<XPUser> xp_users;
        public DbSet<XPQueuedBooster> xp_queued_boosters;
        public DbSet<XPAvailableSlot> xp_available_slots;
        public DbSet<XPActiveBooster> xp_active_boosters;
        public DbSet<XPUserBooster> xp_user_boosters;
    }
}