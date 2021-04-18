using GarbageCan.Domain.Entities.Boosters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityAvailableSlotConfiguration : IEntityTypeConfiguration<EntityAvailableSlot>
    {
        public void Configure(EntityTypeBuilder<EntityAvailableSlot> builder)
        {
            builder.ToTable("xpAvailableSlots");
        }
    }
}