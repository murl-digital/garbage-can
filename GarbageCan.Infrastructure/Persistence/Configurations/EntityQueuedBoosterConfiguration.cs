using GarbageCan.Domain.Entities.Boosters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityQueuedBoosterConfiguration : IEntityTypeConfiguration<EntityQueuedBooster>
    {
        public void Configure(EntityTypeBuilder<EntityQueuedBooster> builder)
        {
            builder.ToTable("xpQueuedBoosters");
        }
    }
}