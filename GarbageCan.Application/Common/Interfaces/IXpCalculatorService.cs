using System.Collections.Generic;
using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IXpCalculatorService
    {
        Task<double> XpEarned(string message, ulong guildId);
        double XpRequired(int level);

        double TotalXpRequired(int lvl)
        {
            var result = 0.0;
            for (var i = 0; i <= lvl; i++)
            {
                result += XpRequired(i);
            }

            return result;
        }
    }
}
