using System.Collections.Generic;
using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IXpCalculatorService
    {
        Task<double> XpEarned(string message, ulong guildId);
        Task<double> XpRequired(int level);

        async Task<double> TotalXpRequired(int lvl)
        {
            var result = 0.0;
            var taskList = new List<Task>();
            for (var i = 0; i <= lvl; i++)
            {
                var j = i;
                taskList.Add(Task.Run(async () => result += await XpRequired(j)));
            }

            await Task.WhenAll(taskList);

            return result;
        }
    }
}
