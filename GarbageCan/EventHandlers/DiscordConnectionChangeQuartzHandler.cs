using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Enums;
using GarbageCan.Domain.Events;
using GarbageCan.Extensions;
using GarbageCan.Jobs;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GarbageCan.EventHandlers
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
                _scheduler.ConfigureJobWithCronSchedule<UnRestrictExpiredChannelRestrictionsJob>(_logger, _configuration, "BackgroundTasks:UnRestrictChannelsCronExpression");
                _scheduler.ConfigureJobWithCronSchedule<UnMuteExpiredMutesJob>(_logger, _configuration, "BackgroundTasks:UnMuteCronExpression");
                _scheduler.ConfigureJobWithCronSchedule<RandomizeStatusJob>(_logger, _configuration,
                    "BackgroundTasks:RandomizeStatusCronExpression");
            }

            return Task.CompletedTask;
        }
    }
}
