using GarbageCan.Application.Common.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure.Services
{
    public class TemplateFileProvider : ITemplateFileProvider
    {
        public TemplateFileProvider()
        {
            using var templateFile = GetStream();

            if (templateFile == null)
                throw new ApplicationException("Asset GarbageCan.Infrastructure.Assets.LvlCommandTemplate.png is missing");
        }

        public Task<Stream> GetTemplateFile()
        {
            return Task.FromResult(GetStream());
        }

        private static Stream GetStream()
        {
            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("GarbageCan.Infrastructure.Assets.LvlCommandTemplate.png");
        }
    }
}