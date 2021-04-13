namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDiscordConfiguration
    {
        public ulong GuildId { get; }
        public string EmojiName { get; }
    }
}