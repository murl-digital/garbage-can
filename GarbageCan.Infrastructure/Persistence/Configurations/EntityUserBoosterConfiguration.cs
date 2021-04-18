using GarbageCan.Domain.Entities.Boosters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityUserBoosterConfiguration : IEntityTypeConfiguration<EntityUserBooster>
    {
        public void Configure(EntityTypeBuilder<EntityUserBooster> builder)
        {
            builder.HasKey(t => t.id);
            builder.ToTable("xpUserBoosters");
        }
    }
}