using DSharpPlus;
using GarbageCan.Application;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Enums;
using GarbageCan.Domain.Events;
using GarbageCan.Infrastructure;
using GarbageCan.WebTest.Configurations;
using GarbageCan.WebTest.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

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

            services.AddApplication();
            services.AddInfrastructure(Configuration);
            services.ConfigureQuartz(Configuration);

            services.Configure<DiscordClientConfiguration>(Configuration.GetSection("DiscordClient"));
            services.AddTransient<IDiscordClientConfiguration>(provider => provider
                .GetRequiredService<IOptions<DiscordClientConfiguration>>()
            .Value);

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

                client.Ready += async (_, _) =>
                {
                    var eventService = provider.GetRequiredService<IDomainEventService>();
                    await eventService.Publish(new DiscordConnectionChangeEvent
                    {
                        Status = DiscordConnectionStatus.Ready
                    });
                };

                return client;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}