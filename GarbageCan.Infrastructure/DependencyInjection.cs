using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Infrastructure.Discord;
using GarbageCan.Infrastructure.Persistence;
using GarbageCan.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using GarbageCan.Infrastructure.Xp;

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
            services.AddTransient<IApplicationDbContext, ApplicationDbContext>();
            services.AddTransient<IDomainEventService, DomainEventService>();
            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<ICurrentUserService, DiscordContextUserService>();

            services.AddTransient<IDiscordGuildService, DiscordGuildService>();
            services.AddTransient<IDiscordResponseService, DiscordResponseService>();
            services.AddTransient<IDiscordGuildRoleService, DiscordGuildRoleService>();
            services.AddScoped<DiscordCommandContextService>();
            services.AddSingleton<DiscordEmojiProviderService>();

            services.AddTransient<IXpCalculatorService, XpCalculatorService>();

            return services;
        }

        public static async Task<IServiceProvider> MigrateAndSeedDatabaseAsync(this IServiceProvider provider)
        {
            await using var context = provider.GetRequiredService<ApplicationDbContext>();
            if (context.Database.IsMySql())
            {
                await context.Database.MigrateAsync();
            }
            await ApplicationDbContextSeed.SeedSampleDataAsync(context);
            return provider;
        }
    }
}