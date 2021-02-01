using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace GarbageCan.Commands.Fun
{
    public class AboutCommandModule : BaseCommandModule
    {
        [Command("about")]
        public async Task About(CommandContext ctx)
        {
            var builder = new DiscordEmbedBuilder();
            builder.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl)
                .WithTitle("Garbage Can")
                .WithDescription($"{Formatter.Italic("A general purpose bot written for the DRACONIUM discord server")} | {Formatter.MaskedUrl("Github", new Uri("https://github.com/SorenNeedsCoffee/garbage-can-csharp"))}");

            builder.AddField("Data Collection",
                $"Garbage Can collects numerical user ids for xp and logging purposes. To see exactly how your data is used, you can check the bot's {Formatter.MaskedUrl("source code", new Uri("https://github.com/SorenNeedsCoffee/garbage-can-csharp"))}");
            
            builder.AddField("Attributions", 
                $"- {Formatter.MaskedUrl(".NET Core 5", new Uri("https://github.com/dotnet/core"))} \n" +
                $"- {Formatter.MaskedUrl("EntityFramework Core", new Uri("https://github.com/dotnet/efcore"))} \n" +
                $"- {Formatter.MaskedUrl("Pomelo.EntityFrameworkCore.MySql", new Uri("https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql"))} \n" +
                $"- {Formatter.MaskedUrl("DSharpPlus", new Uri("https://github.com/DSharpPlus/DSharpPlus"))} \n" +
                $"- SixLabors' {Formatter.MaskedUrl("ImageSharp", new Uri("https://github.com/SixLabors/ImageSharp"))} \n" +
                $"- {Formatter.MaskedUrl("Serilog", new Uri("https://github.com/serilog/serilog"))} \n" +
                $"- {Formatter.MaskedUrl("Config.Net", new Uri("https://github.com/aloneguid/config"))} \n" +
                $"- {Formatter.MaskedUrl("Humanizer", new Uri("https://github.com/Humanizr/Humanizer"))} \n" +
                $"- {Formatter.MaskedUrl("MathNet Numerics", new Uri("https://numerics.mathdotnet.com/"))} \n");

            await ctx.RespondAsync(embed: builder.Build());
        }
    }
}