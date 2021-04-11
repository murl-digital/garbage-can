using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Entities.XP;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<EntityUser> XPUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(builder);
        }
    }
}