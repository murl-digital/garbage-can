using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Domain.Entities.Boosters;
using GarbageCan.Domain.Entities.Config;
using GarbageCan.Domain.Entities.Moderation;
using GarbageCan.Domain.Entities.Presence;
using GarbageCan.Domain.Entities.Roles;
using GarbageCan.Domain.Entities.XP;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
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
        public DbSet<CustomStatus> CustomStatuses { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
