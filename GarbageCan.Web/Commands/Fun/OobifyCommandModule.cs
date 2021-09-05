using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using GarbageCan.Application.Fun.Commands.OobifyText;
using System.Threading.Tasks;

namespace GarbageCan.Web.Commands.Fun
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
