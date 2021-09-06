using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Infrastructure.Discord.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordWebhookService : IDiscordWebhookService
    {
        private readonly DiscordGuildService _guildService;
        private readonly ILogger<DiscordWebhookService> _logger;

        public DiscordWebhookService(DiscordGuildService guildService, ILogger<DiscordWebhookService> logger)
        {
            _guildService = guildService;
            _logger = logger;
        }

        public async Task CreateUserWebhook(string title,
            string description,
            string hookName,
            string avatarUrl,
            ulong guildId,
            ulong channelId)
        {
            try
            {
                var guild = await _guildService.GetGuild(guildId);
                var context = guild.Channels[channelId];
                var webhook = await context.CreateWebhookAsync(hookName);

                var data = new DiscordWebhookBuilder()
                    .WithAvatarUrl(avatarUrl)
                    .AddEmbed(
                        new DiscordEmbedBuilder()
                            .WithColor(new DiscordColor(204, 255, 94))
                            .WithTitle(title)
                            .WithDescription(description)
                    );

                await webhook.ExecuteAsync(data);
                await webhook.DeleteAsync();
            }
            catch (Exception ex)
            {
                if (ex is BadRequestException bad)
                {
                    _logger.LogError(ex, "Failed to create webhook. Errors: {Errors}", bad.Errors);
                }
                else
                {
                    _logger.LogError(ex, "Failed to create webhook");
                }
            }
        }
    }
}
