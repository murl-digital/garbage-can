using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.Boosters;
using GarbageCan.Domain.Entities.Config;
using GarbageCan.Domain.Entities.Moderation;
using GarbageCan.Domain.Entities.Roles;
using GarbageCan.Domain.Entities.XP;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ConditionalRole> ConditionalRoles { get; set; }
        public DbSet<Config> Config { get; set; }
        public DbSet<JoinRole> JoinRoles { get; set; }
        public DbSet<WatchedUser> JoinWatchlist { get; set; }
        public DbSet<LevelRole> LevelRoles { get; set; }
        public DbSet<ActionLog> ModerationActionLogs { get; set; }
        public DbSet<ActiveChannelRestrict> ModerationActiveChannelRestricts { get; set; }
        public DbSet<ActiveMute> ModerationActiveMutes { get; set; }
        public DbSet<ReactionRole> ReactionRoles { get; set; }
        public DbSet<ActiveBooster> XPActiveBoosters { get; set; }
        public DbSet<AvailableSlot> XPAvailableSlots { get; set; }
        public DbSet<ExcludedChannel> XPExcludedChannels { get; set; }
        public DbSet<QueuedBooster> XPQueuedBoosters { get; set; }
        public DbSet<UserBooster> XPUserBoosters { get; set; }
        public DbSet<User> XPUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(builder);
        }
    }
}