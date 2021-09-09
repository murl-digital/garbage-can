using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Domain.Enums;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordPresenceService : IDiscordPresenceService
    {
        private readonly DiscordClient _client;

        public DiscordPresenceService(DiscordClient client)
        {
            _client = client;
        }

        public async Task ChangeStatusAsync(string name, Activity activity)
        {
            if (!Enum.TryParse(activity.ToString(), out ActivityType discordActivity))
                throw new InvalidOperationException("Parsing activity type failed");

            await _client.UpdateStatusAsync(new DiscordActivity(name, discordActivity));
        }
    }
}
