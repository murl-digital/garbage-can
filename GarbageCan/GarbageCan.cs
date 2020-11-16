using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Config.Net;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace GarbageCan
{
	internal static class GarbageCan
	{
		private static DiscordClient _client;
		public static IBotConfig config;

		#region Trap application termination
		[DllImport("Kernel32")]
		private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

		private delegate bool EventHandler(CtrlType sig);
		static EventHandler _handler;

		enum CtrlType {
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT = 1,
			CTRL_CLOSE_EVENT = 2,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT = 6
		}

		private static bool Handler(CtrlType sig) {
			Log.Information("Shutting down...");
			
			_client.UpdateStatusAsync(null, UserStatus.Offline).GetAwaiter().GetResult();
			_client.Dispose();

			//shutdown right away so there are no lingering threads
			Environment.Exit(0);

			return true;
		}
		#endregion

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

			_client = new DiscordClient(new DiscordConfiguration
			{
				Token = config.token,
				TokenType = TokenType.Bot,
				LoggerFactory = logFactory
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

			_client.Ready += (sender, eventArgs) => sender.UpdateStatusAsync(new DiscordActivity("dang"));
			
			_handler += Handler;
			SetConsoleCtrlHandler(_handler, true);

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