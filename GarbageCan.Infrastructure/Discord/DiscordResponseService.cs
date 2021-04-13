using DSharpPlus;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Infrastructure.Discord.Exceptions;
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
    }
}