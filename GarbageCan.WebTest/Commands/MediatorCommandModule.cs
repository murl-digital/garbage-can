using DSharpPlus.CommandsNext;

namespace GarbageCan.WebTest.Commands
{
    public abstract class MediatorCommandModule : BaseCommandModule
    {
        public CommandMediator Mediator { get; set; }
    }
}