using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Config.Net;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GarbageCan
{
	internal static class GarbageCan
	{
		public static DiscordClient Client;
		public static IBotConfig Config;
		
		private static List<IFeature> _botFeatures;
		private static readonly AutoResetEvent Closing = new AutoResetEvent(false);

		private static void Main(string[] args)
		{
			MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		private static async Task MainAsync(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console()
				.CreateLogger();

			var logFactory = new LoggerFactory().AddSerilog();

			BuildConfig();

			Client = new DiscordClient(new DiscordConfiguration
			{
				Token = Config.token,
				TokenType = TokenType.Bot,
				LoggerFactory = logFactory,
				MinimumLogLevel = LogLevel.Debug
			});
			
			_botFeatures = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
				.Where(x => typeof(IFeature).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
				.Select(x => (IFeature)Activator.CreateInstance(x))
				.ToList();

			foreach (var feature in _botFeatures)
			{
				feature.Init(Client);
			}

			_handler += Shutdown;
			
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				SetConsoleCtrlHandler(_handler, true);
			}
			else
			{
				Console.CancelKeyPress += ((sender, eventArgs) => Shutdown(CtrlType.CTRL_C_EVENT));
			}

			await Client.ConnectAsync();
			Closing.WaitOne();
		}

		public static void BuildConfig()
		{
			Config = new ConfigurationBuilder<IBotConfig>()
				.UseJsonFile("dev.json")
				.Build();
			
			if (Config == null) throw new NullReferenceException("Attempted to build config, but got null");
		}
		
		private static bool Shutdown(CtrlType sig)
		{
			Log.Information("Shutting down...");

			foreach (var feature in _botFeatures)
			{
				feature.Cleanup();
			}
			Client.UpdateStatusAsync(null, UserStatus.Offline).GetAwaiter().GetResult();
			Client.Dispose();

			Closing.Set();

			return true;
		}

		#region Windows specific shutdown stuff

		[DllImport("Kernel32")]
		private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

		private delegate bool EventHandler(CtrlType sig);

		private static EventHandler _handler;

		private enum CtrlType
		{
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT = 1,
			CTRL_CLOSE_EVENT = 2,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT = 6
		}
		
		#endregion
	}
}