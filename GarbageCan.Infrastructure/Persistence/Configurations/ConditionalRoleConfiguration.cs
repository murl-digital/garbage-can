using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class ConditionalRoleConfiguration : IEntityTypeConfiguration<ConditionalRole>
    {
        public void Configure(EntityTypeBuilder<ConditionalRole> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("id");
            builder.Property(t => t.GuildId).HasColumnName("guildId");
            builder.Property(t => t.RequiredRoleId).HasColumnName("requiredRoleId");
            builder.Property(t => t.ResultRoleId).HasColumnName("resultRoleId");
            builder.Property(t => t.Remain).HasColumnName("remain");
            builder.ToTable("conditionalRoles");
        }
    }
}
