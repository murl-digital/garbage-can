namespace GarbageCan.Application.Common.Interfaces
{
    public interface IXpCalculatorService
    {
        double XpEarned(string message);
        double XpRequired(int level);

        double TotalXpRequired(int lvl)
        {
            var result = 0.0;
            for (var i = 0; i <= lvl; i++) result += XpRequired(i);

            return result;
        }
    }
}