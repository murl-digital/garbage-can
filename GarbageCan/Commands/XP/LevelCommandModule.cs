using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.XP.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace GarbageCan.Commands.XP
{
    public class LevelCommandModule : BaseCommandModule
    {
        private Font _font;

        public override Task BeforeExecutionAsync(CommandContext ctx)
        {
            try
            {
                if (_font == null)
                {
                    var fontFile = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("GarbageCan.Assets.LvlCommandFont.ttf");

                    if (fontFile == null)
                        throw new ApplicationException("Asset GarbageCan.Assets.LvlCommandFont.ttf is missing");

                    var collection = new FontCollection();
                    var family =
                        collection.Install(fontFile);
                    _font = family.CreateFont(107, FontStyle.Bold);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                if (e.StackTrace != null)
                    Log.Error(e.StackTrace);
            }

            return Task.CompletedTask;
        }

        [Command("level")]
        [Aliases("lvl", "rank")]
        public Task LevelCommand(CommandContext ctx)
        {
            Task.Run(async () =>
            {
                var img = await GenerateImage(ctx.Member);
                await ctx.Channel.SendFileAsync("rank.png", img);
            });

            return Task.CompletedTask;
        }

        [Command("level")]
        public Task LevelCommand(CommandContext ctx, DiscordMember member)
        {
            Task.Run(async () =>
            {
                var img = await GenerateImage(member);
                await ctx.Channel.SendFileAsync("rank.png", img);
            });

            return Task.CompletedTask;
        }

        public async Task<Stream> GenerateImage(DiscordMember discordMember)
        {
            await using var context = new XpContext();
            var user = await context.xpUsers.FirstAsync(u => u.id == discordMember.Id);
            var users = context.xpUsers.ToList();
            users.Sort((a, b) => a.xp.CompareTo(b.xp));
            var placement = users.FindIndex(u => u.id == user.id) + 1;

            var iconPos = new Point(63, 67);
            var usernamePos = new Point(55, 474);
            var lvlPos = new Point(59, 942 - 28);
            var xpPos = new Point(190, 1088 - 28);
            var placementPos = new Point(538, 940 - 28);

            var result = new MemoryStream();

            var templateFile = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("GarbageCan.Assets.LvlCommandTemplate.png");

            if (templateFile == null)
                throw new ApplicationException("Asset GarbageCan.Assets.LvlCommandTemplate.png is missing");

            using var icon = await Image.LoadAsync(new WebClient().OpenRead(discordMember.AvatarUrl));
            using var image = await Image.LoadAsync(templateFile);

            icon.Mutate(i => i.ConvertToAvatar(new Size(401, 401), 200));

            image.Mutate(i => i.DrawImage(icon, iconPos, 1f));

            image.Mutate(i => i.DrawText(discordMember.DisplayName, _font, Color.Black, usernamePos));

            image.Mutate(i => i.DrawText($"{user.lvl}", _font, Color.Black, lvlPos));
            image.Mutate(i => i.DrawText(user.xp.ToString("N1"), _font, Color.Black, xpPos));
            image.Mutate(i => i.DrawText($"{placement}", _font, Color.Black, placementPos));

            await image.SaveAsPngAsync(result);
            result.Seek(0, SeekOrigin.Begin);

            return result;
        }
    }

    //stolen from sixlabors examples, might optimize later
    internal static class ImageProcessingMagic
    {
        internal static void ConvertToAvatar(this IImageProcessingContext processingContext, Size size,
            float cornerRadius)
        {
            processingContext.Resize(new ResizeOptions
            {
                Size = size,
                Mode = ResizeMode.Crop
            }).ApplyRoundedCorners(cornerRadius);
        }


        // This method can be seen as an inline implementation of an `IImageProcessor`:
        // (The combination of `IImageOperations.Apply()` + this could be replaced with an `IImageProcessor`)
        private static void ApplyRoundedCorners(this IImageProcessingContext ctx, float cornerRadius)
        {
            var size = ctx.GetCurrentSize();
            var corners = BuildCorners(size.Width, size.Height, cornerRadius);

            ctx.SetGraphicsOptions(new GraphicsOptions
            {
                Antialias = true,
                AlphaCompositionMode =
                    PixelAlphaCompositionMode
                        .DestOut // enforces that any part of this shape that has color is punched out of the background
            });

            // mutating in here as we already have a cloned original
            // use any color (not Transparent), so the corners will be clipped
            foreach (var c in corners) ctx = ctx.Fill(Color.Red, c);
        }

        private static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            // first create a square
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            // then cut out of the square a circle so we are left with a corner
            var cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the original around the center of the image

            var rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
            var bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;

            // move it across the width of the image - the width of the shape
            var cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            var cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
            var cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }
    }
}