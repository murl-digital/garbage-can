using System;
using System.Threading.Tasks;
using Config.Net;
using DSharpPlus;

namespace GarbageCan
{
    static class GarbageCan
    {
        private static DiscordClient _client;
        public static IBotConfig Config;
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            Config = new ConfigurationBuilder<IBotConfig>()
                .UseJsonFile("dev.json")
                .Build();
            
            Console.WriteLine(Config.Token);
            
            _client = new DiscordClient(new DiscordConfiguration
            {
                Token = Config.Token, //implement this later
                TokenType = TokenType.Bot
            });
            
            //... fun init logic to be done later

            await _client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}