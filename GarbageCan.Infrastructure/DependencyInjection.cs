using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Infrastructure.Discord;
using GarbageCan.Infrastructure.Persistence;
using GarbageCan.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddSingleton<IBoosterService, BoosterService>();

            services.AddTransient<IDiscordGuildRoleService, DiscordGuildRoleService>();
            services.AddTransient<IDiscordGuildChannelService, DiscordGuildChannelService>();
            services.AddTransient<IDiscordModerationService, DiscordModerationService>();
            services.AddTransient<IDiscordDirectMessageService, DiscordDirectMessageService>();
            services.AddTransient<IDiscordMessageService, DiscordMessageService>();
            services.AddTransient<IDiscordWebhookService, DiscordWebhookService>();
            services.AddScoped<DiscordCommandContextService>();
            services.AddSingleton<DiscordEmojiProviderService>();

            services.AddSingleton<ITemplateFileProvider>(new TemplateFileProvider());
            services.AddSingleton<IFontProvider>(new FontProvider());

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
