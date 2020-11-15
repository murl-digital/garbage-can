using System;
using Config.Net;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data
{
    public class XPContext : DbContext
    {
        public DbSet<XPUser> xp_users { get; set; }
        public DbSet<XPQueuedBooster> xp_queued_boosters { get; set; }
        public DbSet<XPAvailableSlot> xp_available_slots { get; set; }
        public DbSet<XPActiveBooster> xp_active_boosters { get; set; }
        public DbSet<XPUserBooster> xp_user_boosters { get; set; }

        private string connectionString;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (GarbageCan.config == null) GarbageCan.BuildConfig();
            Console.WriteLine(GarbageCan.config);
            connectionString = $"host={GarbageCan.config.address};port={GarbageCan.config.port};user id={GarbageCan.config.user};password={GarbageCan.config.password};database={GarbageCan.config.xpSchema}";
            Console.WriteLine(connectionString);
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }
}