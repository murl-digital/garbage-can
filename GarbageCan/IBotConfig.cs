namespace GarbageCan
{
	public interface IBotConfig
	{
		string token { get; }
		string address { get; }
		int port { get; }
		string user { get; }
		string password { get; }
		string xpSchema { get; }
		string commandPrefix { get; }
		
		ulong mutedRoleId { get; }
	}
}