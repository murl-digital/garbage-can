using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class JoinRoleConfiguration : IEntityTypeConfiguration<JoinRole>
    {
        public void Configure(EntityTypeBuilder<JoinRole> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("id");
            builder.Property(t => t.GuildId).HasColumnName("guildId");
            builder.Property(t => t.RoleId).HasColumnName("roleId");
            builder.ToTable("joinRoles");
        }
    }
}
