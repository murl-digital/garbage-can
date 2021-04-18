using GarbageCan.Domain.Entities.XP;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityExcludedChannelConfiguration : IEntityTypeConfiguration<EntityExcludedChannel>
    {
        public void Configure(EntityTypeBuilder<EntityExcludedChannel> builder)
        {
            builder.HasNoKey();
        }
    }
}