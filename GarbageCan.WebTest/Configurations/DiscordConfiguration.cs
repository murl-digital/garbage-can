using GarbageCan.Application.Common.Interfaces;

namespace GarbageCan.WebTest.Configurations
{
    internal class DiscordConfiguration : IDiscordConfiguration
    {
        public ulong GuildId { get; set; }
        public string EmojiName { get; set; }
    }
}