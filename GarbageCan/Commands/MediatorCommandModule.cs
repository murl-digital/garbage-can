using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace GarbageCan.Commands
{
    public abstract class MediatorCommandModule : BaseCommandModule
    {
        public CommandMediator Mediator { get; set; }

        protected static string GetRoleName(DiscordGuild guild, ulong roleId)
        {
            return guild.Roles.ContainsKey(roleId)
                ? guild.Roles[roleId]?.Name
                : string.Empty;
        }

        protected static string GetChannelName(DiscordGuild guild, ulong channelId)
        {
            return guild.Channels.ContainsKey(channelId)
                ? guild.Channels[channelId]?.Name
                : string.Empty;
        }
    }
}
