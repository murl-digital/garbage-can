using System;
using System.Threading.Tasks;
using DSharpPlus;
using GarbageCan.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GarbageCan
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                await MigrateAndSeedDatabase(services, logger);

                await ConnectToDiscord(services, logger);
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) =>
                {
                    configuration.ReadFrom.Configuration(context.Configuration);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var port = Environment.GetEnvironmentVariable("PORT");

                    webBuilder.UseStartup<Startup>()
                        .UseUrls("http://*:" + port);
                });

        private static async Task ConnectToDiscord(IServiceProvider services, ILogger<Program> logger)
        {
            try
            {
                var client = services.GetRequiredService<DiscordClient>();
                await client.ConnectAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while connecting to Discord");
                throw;
            }
        }

        private static async Task MigrateAndSeedDatabase(IServiceProvider services, ILogger<Program> logger)
        {
            try
            {
                await services.MigrateAndSeedDatabaseAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                throw;
            }
        }
    }
}
