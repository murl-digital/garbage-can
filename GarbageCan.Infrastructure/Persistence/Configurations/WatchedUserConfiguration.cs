using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class WatchedUserConfiguration : IEntityTypeConfiguration<WatchedUser>
    {
        public void Configure(EntityTypeBuilder<WatchedUser> builder)
        {
            builder.HasNoKey();
            builder.HasIndex(t => t.GuildId);
            builder.Property(t => t.GuildId).HasColumnName("guildId");
            builder.Property(t => t.UserId).HasColumnName("userId");
            builder.ToTable("joinWatchlist");
        }
    }
}
