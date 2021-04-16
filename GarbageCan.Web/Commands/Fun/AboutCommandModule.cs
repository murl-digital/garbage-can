using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCan.Web.Commands.Fun
{
    public class AboutCommandModule : MediatorCommandModule
    {
        [Command("about")]
        public async Task About(CommandContext ctx)
        {
            var packageStrings = Packages.Select(x => $"- {MaskUrl(x.Key, x.Value)}");
            var combinedPackageStrings = string.Join(Environment.NewLine, packageStrings);

            var builder = new DiscordEmbedBuilder();
            builder.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl).WithTitle("Garbage Can").WithDescription(Description);
            builder.AddField("Data Collection", DataCollection);
            builder.AddField("Attributions", combinedPackageStrings);

            await ctx.RespondAsync(builder.Build());
        }

        private static string MaskUrl(string name, string uriString)
        {
            return Formatter.MaskedUrl(name, new Uri(uriString));
        }

        #region Text

        private static readonly string DataCollection =
            $"Garbage Can collects numerical user ids for xp and logging purposes. To see exactly how your data is used, you can check the bot's {MaskUrl("source code", "https://github.com/SorenNeedsCoffee/garbage-can-csharp")}";

        private static readonly string Description =
            $"{Formatter.Italic("A general purpose bot written for the DRACONIUM discord server")} | {MaskUrl("Github", "https://github.com/murl-digital/garbage-can")}";

        private static readonly Dictionary<string, string> Packages = new Dictionary<string, string>
        {
            { ".NET Core 5", "https://github.com/dotnet/core" },
            { "EntityFramework Core", "https://github.com/dotnet/efcore" },
            { "Pomelo.EntityFrameworkCore.MySql", "https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql" },
            { "DSharpPlus", "https://github.com/DSharpPlus/DSharpPlus" },
            { "MediatR", "https://github.com/jbogard/MediatR" },
            { "FluentValidation", "https://github.com/FluentValidation/FluentValidation" },
            { "AutoMapper", "https://github.com/AutoMapper/AutoMapper" },
            { "ImageSharp", "https://github.com/SixLabors/ImageSharp" },
            { "Serilog", "https://github.com/serilog/serilog" },
            { "Config.Net", "https://github.com/aloneguid/config" },
            { "Humanizer", "https://github.com/Humanizr/Humanizer" },
            { "MathNet Numerics", "https://numerics.mathdotnet.com/" },
            { "Quartz", "https://github.com/quartznet/quartznet" },
        };

        #endregion Text
    }
}