using System;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Moderation.Commands.Restrict;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GarbageCan.Jobs
{
    [DisallowConcurrentExecution]
    public class UnRestrictExpiredChannelRestrictionsJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IDiscordGuildService _guildService;

        public UnRestrictExpiredChannelRestrictionsJob(IMediator mediator,
            IDiscordGuildService guildService,
            ILogger<UnRestrictExpiredChannelRestrictionsJob> logger)
        {
            _logger = logger;
            _mediator = mediator;
            _guildService = guildService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            foreach (var guildId in _guildService.GetAllCurrentGuildIds())
            {
                try
                {
                    await _mediator.Send(new UnRestrictExpiredChannelsCommand
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
