using GarbageCan.Application.Common.Interfaces;

namespace GarbageCan.Web.Configurations
{
    internal class DiscordConfiguration : IDiscordConfiguration
    {
        public string EmojiName { get; set; }
        public string CommandPrefix { get; set; }
    }
}
