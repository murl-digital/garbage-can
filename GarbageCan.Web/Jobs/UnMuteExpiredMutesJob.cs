using System;
using System.Threading.Tasks;
using GarbageCan.Application.Moderation.Commands.Mute;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GarbageCan.Web.Jobs
{
    [DisallowConcurrentExecution]
    public class UnMuteExpiredMutesJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public UnMuteExpiredMutesJob(IMediator mediator, ILogger<UnMuteExpiredMutesJob> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _mediator.Send(new UnMuteExpiredMutesCommand());
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred during a un-restricting channels job");
            }
        }
    }
}