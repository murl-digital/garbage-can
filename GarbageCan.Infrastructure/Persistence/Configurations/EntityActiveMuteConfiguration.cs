using GarbageCan.Domain.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityActiveMuteConfiguration : IEntityTypeConfiguration<EntityActiveMute>
    {
        public void Configure(EntityTypeBuilder<EntityActiveMute> builder)
        {
            builder.HasKey(t => t.id);
            builder.Property(x => x.expirationDate).HasColumnType("datetime");
            builder.ToTable("moderationActiveMutes");
        }
    }
}