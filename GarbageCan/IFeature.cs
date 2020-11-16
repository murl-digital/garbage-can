using DSharpPlus;

namespace GarbageCan
{
	public interface IFeature
	{
		public void init(DiscordClient client);
	}
}