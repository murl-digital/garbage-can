using System;
using GarbageCan.Application.Boosters.Commands;
using GarbageCan.Application.Common.Interfaces;
using MediatR;

namespace GarbageCan.Application.XP.Services
{
    public class XpCalculatorService : IXpCalculatorService
    {
        private readonly IMediator _mediator;

        public XpCalculatorService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public double XpEarned(string message, ulong guildId)
        {
            float GetMultiplier() =>
                _mediator.Send(new GetBoosterMultiplierCommand { GuildId = guildId })
                    .GetAwaiter()
                    .GetResult();

            var length = Math.Sqrt(message.Replace(" ", "").Length);
            length = Math.Min(10, length);

            // TODO: INCOMPLETE - need to implement rng
            // var result = length * (Math.Abs(Random.Sample()) * 0.5 + 1) * BoosterManager.GetMultiplier();
            return length * GetMultiplier();
        }

        public double XpRequired(int level)
        {
            return Math.Round(250 + 75 * Math.Pow(level, 0.6), 1);
        }
    }
}
