using GarbageCan.Domain.Entities.Boosters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class UserBoosterConfiguration : IEntityTypeConfiguration<UserBooster>
    {
        public void Configure(EntityTypeBuilder<UserBooster> builder)
        {
            builder.HasKey(t => t.id);
            builder.ToTable("xpUserBoosters");
        }
    }
}