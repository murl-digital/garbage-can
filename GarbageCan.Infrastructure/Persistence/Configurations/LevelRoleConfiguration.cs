using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class LevelRoleConfiguration : IEntityTypeConfiguration<LevelRole>
    {
        public void Configure(EntityTypeBuilder<LevelRole> builder)
        {
            builder.HasKey(t => t.id);
            builder.ToTable("levelRoles");
        }
    }
}