using GarbageCan.Domain.Entities.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityConfigConfiguration : IEntityTypeConfiguration<EntityConfig>
    {
        public void Configure(EntityTypeBuilder<EntityConfig> builder)
        {
            builder.HasKey(t => t.key);
            builder.ToTable("config");
        }
    }
}