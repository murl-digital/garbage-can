using System;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Moderation.Commands.Mute;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GarbageCan.Jobs
{
    [DisallowConcurrentExecution]
    public class UnMuteExpiredMutesJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IDiscordGuildService _service;

        public UnMuteExpiredMutesJob(IMediator mediator, IDiscordGuildService service, ILogger<UnMuteExpiredMutesJob> logger)
        {
            _logger = logger;
            _mediator = mediator;
            _service = service;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            foreach (var guildId in _service.GetAllCurrentGuildIds())
            {
                try
                {
                    await _mediator.Send(new UnMuteExpiredMutesCommand
                    {
                        GuildId = guildId
                    });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "An error occurred during a un-restricting channels job");
                }
            }
        }
    }
}
