using System;
using Config.Net;
using GarbageCan.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data
{
    public class XPContext : DbContext
    {
        public DbSet<EntityUser> xp_users { get; set; }
        public DbSet<EntityQueuedBooster> xp_queued_boosters { get; set; }
        public DbSet<EntityAvailableSlot> xp_available_slots { get; set; }
        public DbSet<EntityActiveBooster> xp_active_boosters { get; set; }
        public DbSet<EntityUserBooster> xp_user_boosters { get; set; }

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