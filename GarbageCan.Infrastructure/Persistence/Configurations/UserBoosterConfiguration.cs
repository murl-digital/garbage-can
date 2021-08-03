using GarbageCan.Domain.Entities.Boosters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class UserBoosterConfiguration : IEntityTypeConfiguration<UserBooster>
    {
        public void Configure(EntityTypeBuilder<UserBooster> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("id");
            builder.Property(t => t.GuildId).HasColumnName("guildId");
            builder.Property(t => t.UserId).HasColumnName("userId");
            builder.Property(t => t.Multiplier).HasColumnName("multiplier");
            builder.Property(t => t.DurationInSeconds).HasColumnName("durationInSeconds");
            builder.ToTable("xpUserBoosters");
        }
    }
}
