using System.Linq;
using System.Threading.Tasks;
using GarbageCan.Domain.Entities.Roles;
using GarbageCan.Domain.Entities.XP;

namespace GarbageCan.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            // Seed, if necessary
            if (!context.XPUsers.Any())
            {
                await context.XPUsers.AddAsync(new User
                {
                    GuildId = 1,
                    UserId = 39,
                    Lvl = 394,
                    XP = 483
                });
                await context.XPUsers.AddAsync(new User
                {
                    GuildId = 1,
                    UserId = 80,
                    Lvl = 5,
                    XP = 9000
                });
                await context.XPUsers.AddAsync(new User
                {
                    GuildId = 1,
                    UserId = 726179402278371358,
                    Lvl = 5,
                    XP = 5440
                });

                await context.ReactionRoles.AddAsync(new ReactionRole
                {
                    ChannelId = 26121,
                    EmoteId = "12545",
                    Id = 1,
                    MessageId = 4512,
                    RoleId = 1242
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
