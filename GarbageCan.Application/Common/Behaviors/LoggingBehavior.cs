using GarbageCan.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger _logger;

        public LoggingBehavior(ILogger<TRequest> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId ?? string.Empty;

            _logger.LogDebug("SetManagement Request: {Name} {@UserId} {@UserName} {@Request}", requestName, userId, userId, request);
            return Task.CompletedTask;
        }
    }
}