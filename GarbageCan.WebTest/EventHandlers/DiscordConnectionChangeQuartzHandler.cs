using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Enums;
using GarbageCan.Domain.Events;
using GarbageCan.WebTest.Extensions;
using GarbageCan.WebTest.Jobs;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.WebTest.EventHandlers
{
    public class DiscordConnectionChangeQuartzHandler : INotificationHandler<DomainEventNotification<DiscordConnectionChangeEvent>>
    {
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;
        private readonly IConfiguration _configuration;

        public DiscordConnectionChangeQuartzHandler(ILogger<DiscordConnectionChangeQuartzHandler> logger, IScheduler scheduler, IConfiguration configuration)
        {
            _logger = logger;
            _scheduler = scheduler;
            _configuration = configuration;
        }

        public Task Handle(DomainEventNotification<DiscordConnectionChangeEvent> notification, CancellationToken cancellationToken)
        {
            if (notification.DomainEvent.Status == DiscordConnectionStatus.Ready)
            {
                _logger.LogInformation("Starting scheduled jobs");
                _scheduler.ConfigureJobWithCronSchedule<ExampleDiscordJob>(_logger, _configuration, "BackgroundTasks:ExampleJobCronExpression");
            }

            return Task.CompletedTask;
        }
    }
}