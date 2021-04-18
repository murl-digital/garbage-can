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

        public DbSet<EntityUser> XPUsers { get; set; }
        public DbSet<EntityReactionRole> reactionRoles { get; set; }

        public DbSet<EntityConfig> config { get; set; }
        public DbSet<EntityExcludedChannel> xpExcludedChannels { get; set; }
        public DbSet<EntityQueuedBooster> xpQueuedBoosters { get; set; }
        public DbSet<EntityAvailableSlot> xpAvailableSlots { get; set; }
        public DbSet<EntityActiveBooster> xpActiveBoosters { get; set; }
        public DbSet<EntityUserBooster> xpUserBoosters { get; set; }

        public DbSet<EntityActionLog> moderationActionLogs { get; set; }
        public DbSet<EntityActiveMute> moderationActiveMutes { get; set; }
        public DbSet<EntityActiveChannelRestrict> moderationActiveChannelRestricts { get; set; }

        public DbSet<EntityWatchedUser> joinWatchlist { get; set; }
        public DbSet<EntityJoinRole> joinRoles { get; set; }
        public DbSet<EntityConditionalRole> conditionalRoles { get; set; }
        public DbSet<EntityLevelRole> levelRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(builder);
        }
    }
}