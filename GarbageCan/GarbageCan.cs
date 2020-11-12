using System;
using System.Threading.Tasks;
using DSharpPlus;

namespace GarbageCan
{
    class GarbageCan
    {
        private static DiscordClient _client;
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            _client = new DiscordClient(new DiscordConfiguration
            {
                Token = null, //implement this later
                TokenType = TokenType.Bot
            });
            
            //... fun init logic to be done later

            await _client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}