using GarbageCan.Application.Common.Interfaces;

namespace GarbageCan.Configurations
{
    internal class DiscordConfiguration : IDiscordConfiguration
    {
        public string EmojiName { get; set; }
        public string CommandPrefix { get; set; }
    }
}
