using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class ConditionalRoleConfiguration : IEntityTypeConfiguration<ConditionalRole>
    {
        public void Configure(EntityTypeBuilder<ConditionalRole> builder)
        {
            builder.HasKey(t => t.id);
            builder.ToTable("conditionalRoles");
        }
    }
}