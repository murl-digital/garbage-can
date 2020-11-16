using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Config.Net;
using DSharpPlus;

namespace GarbageCan
{
	internal static class GarbageCan
	{
		private static DiscordClient _client;
		public static IBotConfig config;

		private static void Main(string[] args)
		{
			MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		private static async Task MainAsync(string[] args)
		{
			BuildConfig();

			_client = new DiscordClient(new DiscordConfiguration
			{
				Token = config.token, //implement this later
				TokenType = TokenType.Bot
			});

			List<Type> botFeatures = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
				.Where(x => typeof(IFeature).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
				.Select(x => x)
				.ToList();

			foreach (Type t in botFeatures)
			{
				IFeature feature = (IFeature) Activator.CreateInstance(t);
				feature.init(_client);
			}

			await _client.ConnectAsync();
			await Task.Delay(-1);
		}

		public static void BuildConfig()
		{
			config = new ConfigurationBuilder<IBotConfig>()
				.UseJsonFile("dev.json")
				.Build();
		}
	}
}