using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GarbageCan.Web
{
    public class WebAPI : IFeature
    {
        private IHost _hostObj;

        public void Init(DiscordClient client)
        {
            client.Ready += (_, _) =>
            {
                Task.Run(() =>
                {
                    _hostObj = CreateHostBuilder().Build();
                    _hostObj.RunAsync();
                });

                return Task.CompletedTask;
            };
        }

        public void Cleanup()
        {
            _hostObj.StopAsync();
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog();
        }
    }
}