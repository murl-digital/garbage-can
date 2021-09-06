using System.Threading.Tasks;
using DSharpPlus;
using GarbageCan.Application.Boosters.Commands;
using MediatR;
using Quartz;
using Serilog;

namespace GarbageCan.Jobs
{
    public class BoosterCycleJob : IJob
    {
        private readonly IMediator _mediator;
        private readonly DiscordClient _discordClient;
        private readonly ILogger _logger;

        public BoosterCycleJob(IMediator mediator, DiscordClient discordClient, ILogger logger)
        {
            _mediator = mediator;
            _discordClient = discordClient;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.Debug("its booster time");
            foreach (var guildId in _discordClient.Guilds.Keys)
            {
                _logger.Debug("Updating booster cycle for guild {0}", guildId);
                await _mediator.Send(new UpdateBoosterCycleCommand
                {
                    GuildId = guildId
                });
            }
        }
    }
}
