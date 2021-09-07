using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class LevelRoleConfiguration : IEntityTypeConfiguration<LevelRole>
    {
        public void Configure(EntityTypeBuilder<LevelRole> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("id");
            builder.Property(t => t.GuildId).HasColumnName("guildId");
            builder.Property(t => t.RoleId).HasColumnName("roleId");
            builder.Property(t => t.Lvl).HasColumnName("lvl");
            builder.Property(t => t.Remain).HasColumnName("remain");
            builder.ToTable("levelRoles");
        }
    }
}
