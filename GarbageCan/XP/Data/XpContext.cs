using GarbageCan.XP.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.XP.Data
{
	public class XpContext : DbContext
	{
		private string _connectionString;
		public DbSet<EntityUser> xpUsers { get; set; }
		public DbSet<EntityQueuedBooster> xpQueuedBoosters { get; set; }
		public DbSet<EntityAvailableSlot> xpAvailableSlots { get; set; }
		public DbSet<EntityActiveBooster> xpActiveBoosters { get; set; }
		public DbSet<EntityUserBooster> xpUserBoosters { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (GarbageCan.Config == null) GarbageCan.BuildConfig();
			_connectionString =
				$"host={GarbageCan.Config.address};port={GarbageCan.Config.port};user id={GarbageCan.Config.user};password={GarbageCan.Config.password};database={GarbageCan.Config.xpSchema}";
			optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
		}
	}
}