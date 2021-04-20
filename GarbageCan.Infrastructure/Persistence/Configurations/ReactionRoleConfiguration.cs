using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class ReactionRoleConfiguration : IEntityTypeConfiguration<ReactionRole>
    {
        public void Configure(EntityTypeBuilder<ReactionRole> builder)
        {
            builder.ToTable("reactionRoles");
        }
    }
}