using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Data;
using GarbageCan.Data.Entities.XP;
using GarbageCan.XP;
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
    public class LevelCommandModule : BaseCommandModule, IFeature
    {
        private const int BarDifference = 1199;
        private static Font _font;
        private static Font _fontBold;
        private static Font _fontSmall;
        private static Stream _templateFile;

        private static readonly Point IconPos = new(63, 67);
        private static readonly Point LvlPos = new(60, 900);

        private static readonly TextGraphicsOptions Options = new(new GraphicsOptions(), new TextOptions
        {
            DpiX = 1200,
            DpiY = 1200
        });

        private static readonly Point PlacementPos = new(540, 900);
        private static readonly Point UsernamePos = new(55, 485);
        private static readonly Point XpPos = new(187, 1065);

        private static readonly Color Primary = Color.Parse("FCFFFD");
        private static readonly Color Secondary = Color.Parse("B1B3B1");
        private static readonly Color BarColor = Color.Parse("33FE6B");

        public void Init(DiscordClient client)
        {
            var fontFile = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("GarbageCan.Assets.LvlCommandFont.ttf");
            var boldFontFile = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("GarbageCan.Assets.LvlCommandFontBold.ttf");

            if (fontFile == null)
                throw new ApplicationException("Asset GarbageCan.Assets.LvlCommandFont.ttf is missing");
            if (boldFontFile == null)
                throw new ApplicationException("Asset GarbageCan.Assets.LvlCommandFontBold.ttf is missing");

            var collection = new FontCollection();
            _font = collection.Install(fontFile).CreateFont(6.5f, FontStyle.Regular);
            var boldFamily = collection.Install(boldFontFile);
            _fontBold = boldFamily.CreateFont(5.5f, FontStyle.Bold);
            _fontSmall = boldFamily.CreateFont(4.0f, FontStyle.Bold);

            _templateFile = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("GarbageCan.Assets.LvlCommandTemplate.png");

            if (_templateFile == null)
                throw new ApplicationException("Asset GarbageCan.Assets.LvlCommandTemplate.png is missing");
        }

        public void Cleanup()
        {
        }

        [Command("level")]
        [Aliases("lvl", "rank")]
        public Task LevelCommand(CommandContext ctx)
        {
            Task.Run(async () =>
            {
                try
                {
                    await ctx.Channel.TriggerTypingAsync();
                    var img = await GenerateImage(ctx.Member);
                    await ctx.Channel.SendFileAsync("rank.png", img);
                }
                catch (Exception e)
                {
                    Log.Error(e, "LevelCommand failed");
                    await ctx.RespondAsync("there was a problem");
                }
            });

            return Task.CompletedTask;
        }

        [Command("level")]
        public Task LevelCommand(CommandContext ctx, DiscordMember member)
        {
            Task.Run(async () =>
            {
                try
                {
                    await ctx.Channel.TriggerTypingAsync();
                    var img = await GenerateImage(member);
                    await ctx.Channel.SendFileAsync("rank.png", img);
                }
                catch (Exception e)
                {
                    Log.Error(e, "LevelCommand failed");
                    await ctx.RespondAsync("there was a problem");
                }
            });

            return Task.CompletedTask;
        }

        private static async Task<Stream> GenerateImage(DiscordMember discordMember)
        {
            EntityUser user;
            int placement;
            await using (var context = new Context())
            {
                user = await context.xpUsers.FirstAsync(u => u.id == discordMember.Id);
                placement = context.xpUsers
                    .OrderBy(u => u.xp)
                    .Select(u => u.id)
                    .ToList()
                    .FindIndex(u => u == user.id) + 1;
            }

            var currentXp = user.xp - XpManager.TotalXpRequired(user.lvl - 1);
            var required = XpManager.TotalXpRequired(user.lvl);
            var progress = currentXp / XpManager.XpRequired(user.lvl);

            var test = (float) (20 + BarDifference * progress);

            var barPoints = new[]
            {
                new PointF(20, 1229),
                new PointF(20, 1258),
                new PointF(Math.Clamp(test, 20, 1190), 1258),
                new PointF(test, 1229)
            };

            var result = new MemoryStream();

            using var image = await Image.LoadAsync(_templateFile);
            using (var icon = await Image.LoadAsync(new WebClient().OpenRead(discordMember.AvatarUrl)))
            {
                icon.Mutate(i => i.ConvertToAvatar(new Size(401, 401), 200));
                image.Mutate(i => i.DrawImage(icon, IconPos, 1f));
            }

            var glyphs = TextBuilder.GenerateGlyphs(discordMember.DisplayName, UsernamePos,
                new RendererOptions(_fontBold, 1200));

            var widthScale = 763 / glyphs.Bounds.Width;
            var heightScale = 66 / glyphs.Bounds.Height;
            var minScale = Math.Min(widthScale, heightScale);
            glyphs = glyphs.Scale(minScale);
            glyphs = glyphs.Translate(-(glyphs.Bounds.X - UsernamePos.X), 0);

            var discPos = new Point((int) (glyphs.Bounds.Right + 29), 515);

            image.Mutate(i => i.Fill(Primary, glyphs));
            image.Mutate(i => i.DrawText(Options, $"#{discordMember.Discriminator}", _fontSmall, Secondary, discPos));

            image.Mutate(i => i.DrawText(Options, user.lvl.ToString("D"), _font, Primary, LvlPos));
            image.Mutate(i => i.DrawText(Options, placement.ToString("D"), _font, Primary, PlacementPos));

            image.Mutate(i => i.DrawText(Options, $"{user.xp:N0}/{required:N0}", _font, Primary, XpPos));

            var path = new PathBuilder()
                .AddLine(barPoints[0], barPoints[1])
                .AddLine(barPoints[1], barPoints[2])
                .AddLine(barPoints[2], barPoints[3])
                .AddLine(barPoints[3], barPoints[0])
                .Build();

            image.Mutate(i => i.Fill(BarColor, path));

            var (width, height) = image.Size();
            image.Mutate(i => i.Resize(width / 2, height / 2));

            await image.SaveAsPngAsync(result);
            result.Seek(0, SeekOrigin.Begin);
            _templateFile.Seek(0, SeekOrigin.Begin);

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