using GarbageCan.Domain.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class ActiveMuteConfiguration : IEntityTypeConfiguration<ActiveMute>
    {
        public void Configure(EntityTypeBuilder<ActiveMute> builder)
        {
            builder.HasKey(t => t.id);
            builder.Property(x => x.expirationDate).HasColumnType("datetime");
            builder.ToTable("moderationActiveMutes");
        }
    }
}