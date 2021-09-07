using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Infrastructure.Discord;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GarbageCan.Commands
{
    public class CommandMediator
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<CommandMediator> _logger;

        public CommandMediator(IServiceProvider provider,
            ILogger<CommandMediator> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
            CommandContext commandContext,
            CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                using var scope = _provider.CreateScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var contextService = scope.ServiceProvider.GetRequiredService<DiscordCommandContextService>();
                contextService.CommandContext = commandContext;

                return await mediator.Send(request, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred during a command. Request Type: {Request}", request.GetType().Name);
                await commandContext.RespondAsync("An error occurred");
                return default;
            }
        }


        public async Task RespondAsync(CommandContext context, string message, bool prependEmoji = false, bool formatAsBlock = false)
        {
            string content = message;

            if (prependEmoji)
            {
                var config = _provider.GetRequiredService<IDiscordConfiguration>();
                var client = _provider.GetRequiredService<DiscordClient>();
                var discordEmoji = DiscordEmoji.FromName(client, config.EmojiName);
                content = $"{discordEmoji} {message}";
            }

            if (formatAsBlock)
            {
                content = Formatter.BlockCode(content);
            }

            await context.RespondAsync(content);
        }

        public async Task RespondAsync(CommandContext context,
            string fileName,
            Stream stream,
            ulong? replyMessageId = null,
            bool replyMention = false)
        {
            var messageBuilder = new DiscordMessageBuilder().WithFile(fileName, stream);
            if (replyMessageId.HasValue)
            {
                messageBuilder.WithReply(replyMessageId.Value, replyMention);
            }

            await context.RespondAsync(messageBuilder);
        }
    }
}
