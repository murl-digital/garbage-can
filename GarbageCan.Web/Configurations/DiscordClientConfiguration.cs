namespace GarbageCan.Web.Configurations
{
    internal class DiscordClientConfiguration : IDiscordClientConfiguration
    {
        public string CommandPrefix { get; set; }
        public string Token { get; set; }
    }
}