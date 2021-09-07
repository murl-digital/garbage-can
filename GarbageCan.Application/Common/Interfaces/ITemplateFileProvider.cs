using System.IO;
using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface ITemplateFileProvider
    {
        Task<Stream> GetTemplateFile();
    }
}