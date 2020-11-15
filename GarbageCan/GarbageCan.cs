using System;
using System.Threading.Tasks;
using Config.Net;
using DSharpPlus;

namespace GarbageCan
{
    static class GarbageCan
    {
        private static DiscordClient client;
        public static IBotConfig config;
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            BuildConfig();

            client = new DiscordClient(new DiscordConfiguration
            {
                Token = config.token, //implement this later
                TokenType = TokenType.Bot
            });
            
            //... fun init logic to be done later

            await client.ConnectAsync();
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