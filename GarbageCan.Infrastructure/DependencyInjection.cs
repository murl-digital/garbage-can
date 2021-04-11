using GarbageCan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("GarbageCanDbContext"));
            }
            else
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseMySql(
                        connectionString, ServerVersion.AutoDetect(connectionString),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                });
            }
            return services;
        }

        public static async Task<IServiceProvider> MigrateDatabase(this IServiceProvider provider)
        {
            await using var context = provider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
            return provider;
        }
    }
}