using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Infrastructure.Discord.Exceptions;
using System.IO;
using System.Threading.Tasks;

namespace GarbageCan.Infrastructure.Discord
{
    public class DiscordResponseService : IDiscordResponseService
    {
        private readonly DiscordCommandContextService _contextService;
        private readonly DiscordEmojiProviderService _emojiProviderService;

        public DiscordResponseService(DiscordCommandContextService contextService, DiscordEmojiProviderService emojiProviderService)
        {
            _contextService = contextService;
            _emojiProviderService = emojiProviderService;
        }

        public async Task RespondAsync(string message, bool prependEmoji = false, bool formatAsBlock = false)
        {
            if (_contextService.CommandContext == null)
            {
                throw new CommandContextMissingException();
            }

            string content = message;

            if (prependEmoji)
            {
                content = $"{_emojiProviderService.GetEmoji()} {message}";
            }

            if (formatAsBlock)
            {
                content = Formatter.BlockCode(content);
            }

            await _contextService.CommandContext.RespondAsync(content);
        }

        public async Task RespondWithFileAsync(string fileName, Stream image, ulong? replyMessageId = null, bool replyMention = false)
        {
            if (_contextService.CommandContext == null)
            {
                throw new CommandContextMissingException();
            }

            var messageBuilder = new DiscordMessageBuilder().WithFile(fileName, image);
            if (replyMessageId.HasValue)
            {
                messageBuilder.WithReply(replyMessageId.Value, replyMention);
            }

            await _contextService.CommandContext.RespondAsync(messageBuilder);
        }
    }
}