using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityReactionRoleConfiguration : IEntityTypeConfiguration<EntityReactionRole>
    {
        public void Configure(EntityTypeBuilder<EntityReactionRole> builder)
        {
            builder.ToTable("reactionRoles");
        }
    }
}