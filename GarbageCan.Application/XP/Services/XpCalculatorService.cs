using System;
using System.Threading.Tasks;
using GarbageCan.Application.Boosters.Commands;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GarbageCan.Application.XP.Services
{
    public class XpCalculatorService : IXpCalculatorService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<XpCalculatorService> _logger;

        public XpCalculatorService(IMediator mediator, ILogger<XpCalculatorService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<double> XpEarned(string message, ulong guildId)
        {
            var length = Math.Sqrt(message.Replace(" ", "").Length);
            length = Math.Min(10, length);

            // TODO: INCOMPLETE - need to implement rng
            // var result = length * (Math.Abs(Random.Sample()) * 0.5 + 1) * BoosterManager.GetMultiplier();
            var multiplier = await _mediator.Send(new GetBoosterMultiplierCommand { GuildId = guildId });
            _logger.LogDebug("Current multiplier for {GuildId} is {Multiplier}x", guildId, multiplier);
            return length * multiplier;
        }

        public double XpRequired(int level)
        {
            return Math.Round(250 + 75 * Math.Pow(level, 0.6), 1);
        }
    }
}
