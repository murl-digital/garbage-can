using GarbageCan.Domain.Entities.Boosters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class QueuedBoosterConfiguration : IEntityTypeConfiguration<QueuedBooster>
    {
        public void Configure(EntityTypeBuilder<QueuedBooster> builder)
        {
            builder.HasKey(t => t.Id);
            builder.HasIndex(t => t.GuildId);
            builder.Property(t => t.GuildId).HasColumnName("guildId");
            builder.Property(t => t.Position).HasColumnName("position");
            builder.Property(t => t.Multiplier).HasColumnName("multiplier");
            builder.Property(t => t.DurationInSeconds).HasColumnName("durationInSeconds");
            builder.ToTable("xpQueuedBoosters");
        }
    }
}
