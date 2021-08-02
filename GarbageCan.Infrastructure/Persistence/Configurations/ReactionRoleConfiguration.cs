using GarbageCan.Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarbageCan.Infrastructure.Persistence.Configurations
{
    public class ReactionRoleConfiguration : IEntityTypeConfiguration<ReactionRole>
    {
        public void Configure(EntityTypeBuilder<ReactionRole> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("id");
            builder.Property(t => t.GuildId).HasColumnName("guildId");
            builder.Property(t => t.ChannelId).HasColumnName("channelId");
            builder.Property(t => t.MessageId).HasColumnName("messageId");
            builder.Property(t => t.EmoteId).HasColumnName("emoteId");
            builder.Property(t => t.RoleId).HasColumnName("roleId");
            builder.ToTable("reactionRoles");
        }
    }
}
