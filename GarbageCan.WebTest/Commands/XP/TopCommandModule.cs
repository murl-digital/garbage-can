using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using GarbageCan.Application.XP.Commands.PrintTopUsersByXp;
using System.Threading.Tasks;

namespace GarbageCan.WebTest.Commands.XP
{
    public class TopCommandModule : MediatorCommandModule
    {
        [Command("top")]
        public async Task TopCommand(CommandContext ctx)
        {
            await Mediator.Send(new PrintTopUsersByXpCommand { Count = 10, CurrentUserId = ctx.User.Id }, ctx);
        }
    }
}