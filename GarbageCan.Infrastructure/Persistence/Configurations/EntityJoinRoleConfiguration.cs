using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityJoinRoleConfiguration : IEntityTypeConfiguration<EntityJoinRole>
    {
        public void Configure(EntityTypeBuilder<EntityJoinRole> builder)
        {
            builder.HasKey(t => t.id);
            builder.ToTable("joinRoles");
        }
    }
}