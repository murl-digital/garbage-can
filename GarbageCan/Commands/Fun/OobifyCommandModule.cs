using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace GarbageCan.Commands.Fun
{
    public class OobifyCommandModule : BaseCommandModule
    {
        [Command("oobify")]
        public async Task OobifyCommand(CommandContext ctx, string message)
        {
            var result = message
                .Replace("a", "oob")
                .Replace("e", "oob")
                .Replace("i", "oob")
                .Replace("o", "oob")
                .Replace("u", "oob")
                .Replace("A", "Oob")
                .Replace("E", "Oob")
                .Replace("I", "Oob")
                .Replace("O", "Oob")
                .Replace("U", "Oob");

            await ctx.RespondAsync(result);
        }
    }
}