using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GarbageCan.Web
{
    public class Program : IFeature
    {
        public IHost HostObj;

        public void Init(DiscordClient client)
        {
            Task.Run(() =>
            {
                HostObj = CreateHostBuilder().Build();
                HostObj.RunAsync();
            });
        }

        public void Cleanup()
        {
            HostObj.StopAsync();
        }

        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog();
        }
    }
}