using GarbageCan.Domain.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityActionLogConfiguration : IEntityTypeConfiguration<EntityActionLog>
    {
        public void Configure(EntityTypeBuilder<EntityActionLog> builder)
        {
            builder.HasKey(t => t.id);
            builder.Property(x => x.issuedDate).HasColumnType("datetime");
            builder.ToTable("moderationActionLogs");
        }
    }
}