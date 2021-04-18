using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityWatchedUserConfiguration : IEntityTypeConfiguration<EntityWatchedUser>
    {
        public void Configure(EntityTypeBuilder<EntityWatchedUser> builder)
        {
            builder.HasKey(t => t.id);
            builder.ToTable("joinWatchlist");
        }
    }
}