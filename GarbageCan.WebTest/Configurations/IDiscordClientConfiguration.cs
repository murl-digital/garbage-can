namespace GarbageCan.WebTest.Configurations
{
    public interface IDiscordClientConfiguration
    {
        public string Token { get; set; }
    }

    internal class DiscordClientConfiguration : IDiscordClientConfiguration
    {
        public string Token { get; set; }
    }
}