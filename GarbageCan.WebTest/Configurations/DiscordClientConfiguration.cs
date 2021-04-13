namespace GarbageCan.WebTest.Configurations
{
    internal class DiscordClientConfiguration : IDiscordClientConfiguration
    {
        public string CommandPrefix { get; set; }
        public string Token { get; set; }
    }
}