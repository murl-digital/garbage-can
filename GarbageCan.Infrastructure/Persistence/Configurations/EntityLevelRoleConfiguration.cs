using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityLevelRoleConfiguration : IEntityTypeConfiguration<EntityLevelRole>
    {
        public void Configure(EntityTypeBuilder<EntityLevelRole> builder)
        {
            builder.HasKey(t => t.id);
            builder.ToTable("levelRoles");
        }
    }
}