using GarbageCan.Domain.Entities.Roles;
using GarbageCan.Domain.Entities.XP;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            // Seed, if necessary
            if (!context.XPUsers.Any())
            {
                await context.XPUsers.AddAsync(new EntityUser
                {
                    Id = 39,
                    Lvl = 394,
                    XP = 483
                });
                await context.XPUsers.AddAsync(new EntityUser
                {
                    Id = 80,
                    Lvl = 5,
                    XP = 9000
                });
                await context.XPUsers.AddAsync(new EntityUser
                {
                    Id = 726179402278371358,
                    Lvl = 5,
                    XP = 5440
                });

                await context.reactionRoles.AddAsync(new EntityReactionRole()
                {
                    channelId = 26121,
                    emoteId = "12545",
                    id = 1,
                    messageId = 4512,
                    roleId = 1242
                });

                await context.SaveChangesAsync();
            }
        }
    }
}