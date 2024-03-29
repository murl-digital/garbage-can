using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using GarbageCan.Application;
using GarbageCan.Application.Common.Configuration;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Commands;
using GarbageCan.Commands.Fun;
using GarbageCan.Configurations;
using GarbageCan.Domain.Common;
using GarbageCan.Domain.Entities;
using GarbageCan.Domain.Enums;
using GarbageCan.Domain.Events;
using GarbageCan.Extensions;
using GarbageCan.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using DiscordConfiguration = GarbageCan.Configurations.DiscordConfiguration;

namespace GarbageCan
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

            services.AddApplication(typeof(Startup).Assembly);
            services.AddInfrastructure(Configuration);

            services.ConfigureQuartz(Configuration);

            services.Configure<IDiscordClientConfiguration, DiscordClientConfiguration>(
                Configuration.GetSection("Discord:Client"));
            services.Configure<IDiscordConfiguration, DiscordConfiguration>(
                Configuration.GetSection("Discord"));
            services.Configure<IRoleConfiguration, RoleConfiguration>(Configuration.GetSection("Roles"));

            services.AddTransient<CommandMediator>();

            services.AddSingleton<DiscordClient>(provider =>
            {
                var clientConfiguration = provider.GetRequiredService<IDiscordClientConfiguration>();
                var configuration = provider.GetRequiredService<IDiscordConfiguration>();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var client = new DiscordClient(new DSharpPlus.DiscordConfiguration
                {
                    Token = clientConfiguration.Token,
                    TokenType = TokenType.Bot,
                    LoggerFactory = loggerFactory,
                    MinimumLogLevel = LogLevel.Debug,
                    Intents = DiscordIntents.All
                });

                var commands = client.UseCommandsNext(new CommandsNextConfiguration
                {
                    StringPrefixes = new[] { configuration.CommandPrefix },
                    Services = provider
                });

                commands.RegisterCommands(Assembly.GetExecutingAssembly());
                commands.RegisterConverter(new ActivityConverter());

                client.Ready += async (_, _) =>
                {
                    await PublishScopedEvent(provider, new DiscordConnectionChangeEvent
                    {
                        Status = DiscordConnectionStatus.Ready
                    });
                };

                client.MessageCreated += async (_, args) =>
                {
                    await PublishScopedEvent(provider, new DiscordMessageCreatedEvent
                    {
                        AuthorId = args.Author.Id,
                        GuildId = args.Guild.Id,
                        ChannelId = args.Channel.Id,
                        MessageId = args.Message.Id,
                        Content = args.Message.Content,
                        AuthorIsBot = args.Author.IsBot,
                        AuthorIsSystem = args.Author.IsSystem ?? false,
                        ChannelIsPrivate = args.Channel.IsPrivate,
                        AuthorAvatarUrl = args.Author.AvatarUrl,
                        AuthorDisplayName = (await args.Guild.GetMemberAsync(args.Author.Id))?.DisplayName
                    });
                };

                client.MessageReactionAdded += async (_, args) =>
                {
                    await PublishScopedEvent(provider, new DiscordMessageReactionAddedEvent
                    {
                        ChannelId = args.Channel.Id,
                        Emoji = new Emoji
                        {
                            Id = args.Emoji.Id,
                            Name = args.Emoji.Name
                        },
                        GuildId = args.Guild.Id,
                        MessageId = args.Message.Id,
                        UserId = args.User.Id,
                    });
                };

                client.MessageReactionRemoved += async (_, args) =>
                {
                    await PublishScopedEvent(provider, new DiscordMessageReactionRemovedEvent
                    {
                        ChannelId = args.Channel.Id,
                        Emoji = new Emoji
                        {
                            Id = args.Emoji.Id,
                            Name = args.Emoji.Name
                        },
                        GuildId = args.Guild.Id,
                        MessageId = args.Message.Id,
                        UserId = args.User.Id
                    });
                };

                client.GuildMemberAdded += async (_, args) =>
                {
                    await PublishScopedEvent(provider, new DiscordGuildMemberAdded
                    {
                        GuildId = args.Guild.Id,
                        UserId = args.Member.Id,
                        IsBot = args.Member.IsBot
                    });
                };

                client.GuildMemberUpdated += async (_, args) =>
                {
                    await PublishScopedEvent(provider, new DiscordGuildMemberUpdated
                    {
                        GuildId = args.Guild.Id,
                        UserId = args.Member.Id,
                        IsBot = args.Member.IsBot,
                        IsPending = args.Member.IsPending
                    });
                };

                client.GuildDownloadCompleted += async (_, _) =>
                {
                    await PublishScopedEvent(provider, new DiscordGuildDownloadCompleteEvent());
                };

                client.GuildUpdated += async (_, args) =>
                {
                    await PublishScopedEvent(provider, new DiscordGuildUpdatedEvent
                    {
                        GuildId = args.GuildBefore.Id,
                        PreviousBoostCount = args.GuildBefore.PremiumSubscriptionCount ?? 0,
                        BoostCount = args.GuildAfter.PremiumSubscriptionCount ?? 0
                    });
                };

                return client;
            });

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Garbage Can",
                    Description = "REST api to interact with the Garbage Can Discord Bot",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Joe Sorensen",
                        Email = "no peeking",
                        Url = new Uri("https://github.com/murl-digital")
                    }
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("gbc",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .Build()
                );
            });

            AddRateLimiting(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GarbageCan v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("gbc");

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            applicationLifetime.ApplicationStopped.Register(() =>
            {
                var service = app.ApplicationServices.GetService<IDomainEventService>();
                var logger = app.ApplicationServices.GetService<ILogger<Startup>>();

                logger.LogInformation("SHUTTING DOWN");
                service?.Publish(new DiscordConnectionChangeEvent { Status = DiscordConnectionStatus.Shutdown })
                    .GetAwaiter().GetResult();

                var client = app.ApplicationServices.GetService<DiscordClient>();
                client?.DisconnectAsync().GetAwaiter().GetResult();

                logger.LogInformation("SHUT DOWN");
            });
        }

        private static async Task PublishScopedEvent(IServiceProvider provider, DomainEvent notification)
        {
            var logger = provider.GetRequiredService<ILogger<Startup>>();
            try
            {
                using var scope = provider.CreateScope();
                var eventService = scope.ServiceProvider.GetRequiredService<IDomainEventService>();
                await eventService.Publish(notification);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error On Scoped event publish. Event: {@Event}", notification);
            }
        }

        private static void AddRateLimiting(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(options =>
            {
                options.EnableEndpointRateLimiting = true;
                options.StackBlockedRequests = true;
                options.GeneralRules = new List<RateLimitRule>
                {
                    new()
                    {
                        Endpoint = "*",
                        Period = "30s",
                        Limit = 20
                    }
                };
            });
            services.Configure<IpRateLimitPolicies>(_ => { });
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddHttpContextAccessor();
        }
    }
}
