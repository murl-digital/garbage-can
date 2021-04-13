using DSharpPlus.CommandsNext;
using GarbageCan.Infrastructure.Discord;
using GarbageCan.WebTest.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.WebTest.Commands
{
    public class CommandMediator
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<CommandMediator> _logger;

        public CommandMediator(IServiceProvider provider, ILogger<CommandMediator> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request,
            CommandContext commandContext,
            CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                using var scope = _provider.CreateScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var contextService = scope.ServiceProvider.GetRequiredService<DiscordCommandContextService>();
                contextService.CommandContext = commandContext;

                var result = await mediator.Send(request, cancellationToken);

                return Result<TResponse>.Success(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred during a command. Request Type: {Request}", request.GetType().Name);
                await commandContext.RespondAsync("An error occurred");
                return Result<TResponse>.Failure(new[] { exception.Message });
            }
        }
    }
}