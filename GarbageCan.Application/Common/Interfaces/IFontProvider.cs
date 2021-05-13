using GarbageCan.Application.Common.Models;
using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IFontProvider
    {
        Task<Fonts> GetFontsAsync();
    }
}