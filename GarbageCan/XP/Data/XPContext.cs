using GarbageCan.XP.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.XP.Data
{
	public class XPContext : DbContext
	{
		private string _connectionString;
		public DbSet<EntityUser> xpUsers { get; set; }
		public DbSet<EntityQueuedBooster> xpQueuedBoosters { get; set; }
		public DbSet<EntityAvailableSlot> xpAvailableSlots { get; set; }
		public DbSet<EntityActiveBooster> xpActiveBoosters { get; set; }
		public DbSet<EntityUserBooster> xpUserBoosters { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (GarbageCan.config == null) GarbageCan.BuildConfig();
			_connectionString =
				$"host={GarbageCan.config.address};port={GarbageCan.config.port};user id={GarbageCan.config.user};password={GarbageCan.config.password};database={GarbageCan.config.xpSchema}";
			optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
		}
	}
}