using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityConditionalRoleConfiguration : IEntityTypeConfiguration<EntityConditionalRole>
    {
        public void Configure(EntityTypeBuilder<EntityConditionalRole> builder)
        {
            builder.HasKey(t => t.id);
            builder.ToTable("conditionalRoles");
        }
    }
}