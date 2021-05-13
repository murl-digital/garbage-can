using GarbageCan.Application.Common.Interfaces;
using MediatR;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.XP.Queries.GetXPImageStream
{
    public class GetXPImageStreamQuery : IRequest<Stream>
    {
        public string AvatarUrl { get; set; }
        public string Discriminator { get; set; }
        public string DisplayName { get; set; }
        public int Level { get; set; }
        public int Placement { get; set; }
        public double Progress { get; set; }
        public double Required { get; set; }
        public double Xp { get; set; }
    }

    public class GetXPImageStreamQueryHandler : IRequestHandler<GetXPImageStreamQuery, Stream>
    {
        private static readonly Color BarColor = Color.Parse("33FE6B");
        private static readonly Point IconPos = new Point(63, 67);
        private static readonly Point LvlPos = new Point(60, 900);

        private static readonly TextGraphicsOptions Options = new TextGraphicsOptions(new GraphicsOptions(), new TextOptions
        {
            DpiX = 1200,
            DpiY = 1200
        });

        private static readonly Point PlacementPos = new Point(540, 900);
        private static readonly Color Primary = Color.Parse("FCFFFD");
        private static readonly Color Secondary = Color.Parse("B1B3B1");
        private static readonly Point UsernamePos = new Point(55, 485);
        private static readonly Point XpPos = new Point(187, 1065);
        private readonly IFontProvider _fontProvider;
        private readonly ITemplateFileProvider _templateFileProvider;

        public GetXPImageStreamQueryHandler(IFontProvider fontProvider, ITemplateFileProvider templateFileProvider)
        {
            _fontProvider = fontProvider;
            _templateFileProvider = templateFileProvider;
        }

        public async Task<Stream> Handle(GetXPImageStreamQuery request, CancellationToken cancellationToken)
        {
            var fonts = await _fontProvider.GetFontsAsync();

            int BarDifference = 1199;
            var test = (float)(20 + BarDifference * request.Progress);
            var barPoints = new[]
            {
                new PointF(20, 1229),
                new PointF(20, 1258),
                new PointF(Math.Clamp(test, 20, 1190), 1258),
                new PointF(test, 1229)
            };
            var result = new MemoryStream();

            var templateFile = await _templateFileProvider.GetTemplateFile();
            using var image = await Image.LoadAsync(templateFile);
            using (var icon = await Image.LoadAsync(new WebClient().OpenRead(request.AvatarUrl)))
            {
                icon.Mutate(i => i.ConvertToAvatar(new Size(401, 401), 200));
                image.Mutate(i => i.DrawImage(icon, IconPos, 1f));
            }

            var glyphs = TextBuilder.GenerateGlyphs(request.DisplayName, UsernamePos,
                    new RendererOptions(fonts.Bold, 1200));

            var widthScale = 763 / glyphs.Bounds.Width;
            var heightScale = 66 / glyphs.Bounds.Height;
            var minScale = Math.Min(widthScale, heightScale);
            glyphs = glyphs.Scale(minScale);
            glyphs = glyphs.Translate(-(glyphs.Bounds.X - UsernamePos.X), 0);

            var discPos = new Point((int)(glyphs.Bounds.Right + 29), 515);

            image.Mutate(i => i.Fill(Primary, glyphs));
            image.Mutate(i => i.DrawText(Options, $"#{request.Discriminator}", fonts.Small, Secondary, discPos));

            image.Mutate(i => i.DrawText(Options, request.Level.ToString("D"), fonts.Normal, Primary, LvlPos));
            image.Mutate(i => i.DrawText(Options, request.Placement.ToString("D"), fonts.Normal, Primary, PlacementPos));

            image.Mutate(i => i.DrawText(Options, $"{request.Xp:N0}/{request.Required:N0}", fonts.Normal, Primary, XpPos));

            var path = new PathBuilder()
                .AddLine(barPoints[0], barPoints[1])
                .AddLine(barPoints[1], barPoints[2])
                .AddLine(barPoints[2], barPoints[3])
                .AddLine(barPoints[3], barPoints[0])
                .Build();

            image.Mutate(i => i.Fill(BarColor, path));

            var (width, height) = image.Size();
            image.Mutate(i => i.Resize(width / 2, height / 2));

            await image.SaveAsPngAsync(result, cancellationToken);
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