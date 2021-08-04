using DSharpPlus.CommandsNext;

namespace GarbageCan.Web.Commands
{
    public abstract class MediatorCommandModule : BaseCommandModule
    {
        public CommandMediator Mediator { get; set; }
    }
}
