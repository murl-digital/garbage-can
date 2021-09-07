using GarbageCan.Domain.Entities.XP;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class ExcludedChannelConfiguration : IEntityTypeConfiguration<ExcludedChannel>
    {
        public void Configure(EntityTypeBuilder<ExcludedChannel> builder)
        {
            builder.HasNoKey();
            builder.Property(t => t.GuildId).HasColumnName("guildId");
            builder.Property(t => t.ChannelId).HasColumnName("channelId");
            builder.ToTable("xpExcludedChannels");
        }
    }
}
