using GarbageCan.Domain.Entities.Boosters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class AvailableSlotConfiguration : IEntityTypeConfiguration<AvailableSlot>
    {
        public void Configure(EntityTypeBuilder<AvailableSlot> builder)
        {
            builder.HasKey(t => t.id);
            builder.ToTable("xpAvailableSlots");
        }
    }
}