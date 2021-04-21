using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class WatchedUserConfiguration : IEntityTypeConfiguration<WatchedUser>
    {
        public void Configure(EntityTypeBuilder<WatchedUser> builder)
        {
            builder.HasKey(t => t.id);
            builder.ToTable("joinWatchlist");
        }
    }
}