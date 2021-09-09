using GarbageCan.Domain.Entities.Presence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class CustomStatusConfiguration : IEntityTypeConfiguration<CustomStatus>
    {
        public void Configure(EntityTypeBuilder<CustomStatus> builder)
        {
            builder.ToTable("customStatuses");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Activity).HasColumnName("activity");
            builder.Property(t => t.Name).HasColumnName("name");
        }
    }
}
