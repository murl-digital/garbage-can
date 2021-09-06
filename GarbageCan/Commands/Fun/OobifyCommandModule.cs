using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using GarbageCan.Application.Fun.Commands.OobifyText;

namespace GarbageCan.Commands.Fun
{
    public class OobifyCommandModule : MediatorCommandModule
    {
        [Command("oobify")]
        public async Task OobifyCommand(CommandContext ctx, [RemainingText] string message)
        {
            var text = await Mediator.Send(new OobifyTextCommand { Text = message }, ctx);
            await Mediator.RespondAsync(ctx, text);
        }
    }
}
