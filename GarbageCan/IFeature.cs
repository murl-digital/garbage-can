using DSharpPlus;

namespace GarbageCan
{
    public interface IFeature
    {
        public void Init(DiscordClient client);
        public void Cleanup();
    }
}