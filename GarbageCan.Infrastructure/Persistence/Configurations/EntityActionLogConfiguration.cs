using GarbageCan.Domain.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityActionLogConfiguration : IEntityTypeConfiguration<EntityActionLog>
    {
        public void Configure(EntityTypeBuilder<EntityActionLog> builder)
        {
            builder.ToTable("moderationActionLogs");
        }
    }
}