using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GarbageCan.Application.XP.Commands.PrintTopUsersByXp;
using GarbageCan.Application.XP.Commands.PrintUserLevel;
using System.Threading.Tasks;

namespace GarbageCan.Web.Commands.XP
{
    public class XpCommandModule : MediatorCommandModule
    {
        [Command("level")]
        [Aliases("lvl", "rank")]
        public async Task LevelCommand(CommandContext ctx)
        {
            await PrintUserLevelCommand(ctx, ctx.Member);
        }

        [Command("level")]
        public async Task LevelCommand(CommandContext ctx, DiscordMember member)
        {
            await PrintUserLevelCommand(ctx, member);
        }

        [Command("top")]
        public async Task TopCommand(CommandContext ctx)
        {
            await Mediator.Send(new PrintTopUsersByXpCommand { Count = 10, CurrentUserId = ctx.User.Id }, ctx);
        }

        private async Task PrintUserLevelCommand(CommandContext ctx, DiscordMember member)
        {
            await ctx.Channel.TriggerTypingAsync();
            await Mediator.Send(new PrintUserLevelCommand
            {
                UserId = member.Id,
                AvatarUrl = member.AvatarUrl,
                MessageId = ctx.Message.Id,
                MemberDiscriminator = member.Discriminator
            }, ctx);
        }
    }
}