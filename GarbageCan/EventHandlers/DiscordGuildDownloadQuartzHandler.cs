using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Events;
using GarbageCan.Extensions;
using GarbageCan.Jobs;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GarbageCan.EventHandlers
{
    public class DiscordGuildDownloadQuartzHandler : INotificationHandler<DomainEventNotification<DiscordGuildDownloadCompleteEvent>>
    {
        private readonly ILogger<DiscordGuildDownloadQuartzHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly IScheduler _scheduler;
        private readonly IDiscordGuildRoleService _roleService;

        public DiscordGuildDownloadQuartzHandler(ILogger<DiscordGuildDownloadQuartzHandler> logger, IConfiguration configuration, IScheduler scheduler, IDiscordGuildRoleService roleService)
        {
            _logger = logger;
            _configuration = configuration;
            _scheduler = scheduler;
            _roleService = roleService;
        }
        public async Task Handle(DomainEventNotification<DiscordGuildDownloadCompleteEvent> notification, CancellationToken cancellationToken)
        {
            _scheduler.ConfigureJobWithCronSchedule<ApplyConditionalRolesJob>(_logger, _configuration, "BackgroundTasks:ApplyConditionalRolesCronExpression");
            _scheduler.ConfigureJobWithCronSchedule<BoosterCycleJob>(_logger, _configuration,
                "BackgroundTasks:BoosterCycleCronExpression");
        }
    }
}
