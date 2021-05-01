using GarbageCan.Application.Moderation.Commands.Restrict;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace GarbageCan.Web.Jobs
{
    [DisallowConcurrentExecution]
    public class UnRestrictExpiredChannelRestrictionsJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public UnRestrictExpiredChannelRestrictionsJob(IMediator mediator, ILogger<UnRestrictExpiredChannelRestrictionsJob> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _mediator.Send(new UnRestrictExpiredChannelsCommand());
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred during a un-restricting channels job");
            }
        }
    }
}