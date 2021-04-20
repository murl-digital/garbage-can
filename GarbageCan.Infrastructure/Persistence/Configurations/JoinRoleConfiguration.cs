using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class JoinRoleConfiguration : IEntityTypeConfiguration<JoinRole>
    {
        public void Configure(EntityTypeBuilder<JoinRole> builder)
        {
            builder.HasKey(t => t.id);
            builder.ToTable("joinRoles");
        }
    }
}