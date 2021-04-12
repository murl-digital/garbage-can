using GarbageCan.Application.XP.Queries.GetTopUsersByXP;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace GarbageCan.WebTest.Jobs
{
    [DisallowConcurrentExecution]
    public class ExampleDiscordJob : IJob
    {
        private readonly ILogger<ExampleDiscordJob> _logger;
        private readonly IMediator _mediator;

        public ExampleDiscordJob(ILogger<ExampleDiscordJob> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var topTenUsers = await _mediator.Send(new GetTopUsersByXPQuery { Count = 10, CurrentUserId = 39 });
            _logger.LogInformation("Top Users: {@ViewModel}", topTenUsers);
        }
    }
}