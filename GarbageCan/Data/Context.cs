﻿using GarbageCan.Data.Entities.Boosters;
using GarbageCan.Data.Entities.Config;
using GarbageCan.Data.Entities.Moderation;
using GarbageCan.Data.Entities.Roles;
using GarbageCan.Data.Entities.XP;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data
{
    public class Context : DbContext
    {
        private string _connectionString;

        public DbSet<EntityConfig> config { get; set; }

        public DbSet<EntityUser> xpUsers { get; set; }
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
        public DbSet<EntityReactionRole> reactionRoles { get; set; }
        public DbSet<EntityConditionalRole> conditionalRoles { get; set; }
        public DbSet<EntityLevelRole> levelRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (GarbageCan.Config == null) GarbageCan.BuildConfig();
            _connectionString ??=
                $"host={GarbageCan.Config.address};port={GarbageCan.Config.port};user id={GarbageCan.Config.user};password={GarbageCan.Config.password};database={GarbageCan.Config.xpSchema}";

            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
        }
    }
}