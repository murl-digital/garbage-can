using GarbageCan.Domain.Entities.XP;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("id");
            builder.Property(t => t.GuildId).HasColumnName("guildId");
            builder.Property(t => t.UserId).ValueGeneratedNever().HasColumnName("userId");
            builder.Property(t => t.XP).HasColumnName("xp");
            builder.Property(t => t.Lvl).HasColumnName("lvl");
            builder.ToTable("xpUsers");
        }
    }
}