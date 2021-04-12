namespace GarbageCan.WebTest.Configurations
{
    public interface IDiscordClientConfiguration
    {
        public string CommandPrefix { get; set; }
        public string Token { get; set; }
    }

    internal class DiscordClientConfiguration : IDiscordClientConfiguration
    {
        public string CommandPrefix { get; set; }
        public string Token { get; set; }
    }
}