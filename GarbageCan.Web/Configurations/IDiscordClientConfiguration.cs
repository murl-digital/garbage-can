namespace GarbageCan.Web.Configurations
{
    public interface IDiscordClientConfiguration
    {
        public string CommandPrefix { get; }
        public string Token { get;  }
    }
}