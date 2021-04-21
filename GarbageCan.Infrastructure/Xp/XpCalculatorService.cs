using System;
using GarbageCan.Application.Common.Interfaces;

namespace GarbageCan.Infrastructure.Xp
{
    public class XpCalculatorService : IXpCalculatorService
    {
        public double XpEarned(string message)
        {
            var length = Math.Sqrt(message.Replace(" ", "").Length);
            length = Math.Min(10, length);

            // TODO: INCOMPLETE - need to implement boosters and rng
            // var result = length * (Math.Abs(Random.Sample()) * 0.5 + 1) * BoosterManager.GetMultiplier();
            var result = length;
            return result;
        }

        public double XpRequired(int level)
        {
            return Math.Round(250 + 75 * Math.Pow(level, 0.6), 1);
        }
    }
}