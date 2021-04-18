using GarbageCan.Domain.Entities.Boosters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class EntityActiveBoosterConfiguration : IEntityTypeConfiguration<EntityActiveBooster>
    {
        public void Configure(EntityTypeBuilder<EntityActiveBooster> builder)
        {
            // Creates a shadow property for the foriegn key relationship
            // https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key#shadow-foreign-key
            builder.Property<int>("id");
            builder.HasOne(x => x.slot).WithMany().HasForeignKey("id");

            builder.Property(x => x.expirationDate).HasColumnType("datetime");
            builder.ToTable("xpActiveBoosters");
        }
    }
}