using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;
using SixLabors.Fonts;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure.Services
{
    public class FontProvider : IFontProvider
    {
        public FontProvider()
        {
            using var fontFile = GetFontFile();
            using var boldFontFile = GetBoldFontFile();

            if (fontFile == null)
                throw new ApplicationException("Asset GarbageCan.Infrastructure.Assets.LvlCommandFont.ttf is missing");
            if (boldFontFile == null)
                throw new ApplicationException("Asset GarbageCan.Infrastructure.Assets.LvlCommandFontBold.ttf is missing");
        }

        public Task<Fonts> GetFontsAsync()
        {
            using var fontFile = GetFontFile();
            using var boldFontFile = GetBoldFontFile();
            var collection = new FontCollection();
            var font = collection.Install(fontFile).CreateFont(6.5f, FontStyle.Regular);
            var boldFamily = collection.Install(boldFontFile);
            var fontBold = boldFamily.CreateFont(5.5f, FontStyle.Bold);
            var fontSmall = boldFamily.CreateFont(4.0f, FontStyle.Bold);

            return Task.FromResult(new Fonts { Normal = font, Bold = fontBold, Small = fontSmall });
        }

        private static Stream GetBoldFontFile()
        {
            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("GarbageCan.Infrastructure.Assets.LvlCommandFontBold.ttf");
        }

        private static Stream GetFontFile()
        {
            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("GarbageCan.Infrastructure.Assets.LvlCommandFont.ttf");
        }
    }
}