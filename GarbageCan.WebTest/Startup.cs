using DSharpPlus;
using DSharpPlus.CommandsNext;
using GarbageCan.Application;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Enums;
using GarbageCan.Domain.Events;
using GarbageCan.Infrastructure;
using GarbageCan.WebTest.Commands;
using GarbageCan.WebTest.Configurations;
using GarbageCan.WebTest.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using DiscordConfiguration = DSharpPlus.DiscordConfiguration;

namespace GarbageCan.WebTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GarbageCan.WebTest", Version = "v1" });
            });

            services.AddApplication(typeof(Startup).Assembly);
            services.AddInfrastructure(Configuration);

            services.ConfigureQuartz(Configuration);

            services.Configure<IDiscordClientConfiguration, DiscordClientConfiguration>(Configuration.GetSection("Discord:Client"));
            services.Configure<IDiscordConfiguration, Configurations.DiscordConfiguration>(Configuration.GetSection("Discord"));

            services.AddTransient<CommandMediator>();

            services.AddSingleton<DiscordClient>(provider =>
            {
                var config = provider.GetRequiredService<IDiscordClientConfiguration>();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var client = new DiscordClient(new DiscordConfiguration
                {
                    Token = config.Token,
                    TokenType = TokenType.Bot,
                    LoggerFactory = loggerFactory,
                    MinimumLogLevel = LogLevel.Debug,
                    Intents = DiscordIntents.All
                });

                var commands = client.UseCommandsNext(new CommandsNextConfiguration
                {
                    StringPrefixes = new[] { config.CommandPrefix },
                    Services = provider
                });

                commands.RegisterCommands(Assembly.GetExecutingAssembly());

                client.Ready += async (_, _) =>
                {
                    var logger = provider.GetRequiredService<ILogger<Startup>>();
                    try
                    {
                        logger.LogInformation("Discord connection is {Status}", DiscordConnectionStatus.Ready);
                        var eventService = provider.GetRequiredService<IDomainEventService>();
                        await eventService.Publish(new DiscordConnectionChangeEvent
                        {
                            Status = DiscordConnectionStatus.Ready
                        });
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception, "Error On Ready");
                    }
                };

                return client;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GarbageCan.WebTest v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            applicationLifetime.ApplicationStopped.Register(() =>
            {
                var service = app.ApplicationServices.GetService<IDomainEventService>();
                var logger = app.ApplicationServices.GetService<ILogger<Startup>>();

                logger.LogInformation("SHUTTING DOWN");
                service?.Publish(new DiscordConnectionChangeEvent { Status = DiscordConnectionStatus.Shutdown }).GetAwaiter().GetResult();

                var client = app.ApplicationServices.GetService<DiscordClient>();
                client?.DisconnectAsync().GetAwaiter().GetResult();

                logger.LogInformation("SHUT DOWN");
            });
        }
    }
}