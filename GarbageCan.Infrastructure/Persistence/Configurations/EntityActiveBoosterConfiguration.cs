using GarbageCan.Domain.Entities.Boosters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityActiveBoosterConfiguration : IEntityTypeConfiguration<EntityActiveBooster>
    {
        public void Configure(EntityTypeBuilder<EntityActiveBooster> builder)
        {
            builder.ToTable("xpActiveBoosters");
        }
    }
}