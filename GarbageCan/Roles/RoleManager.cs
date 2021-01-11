using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using GarbageCan.Data;
using GarbageCan.XP;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GarbageCan.Roles
{
    public class RoleManager : IFeature
    {
        public void Init(DiscordClient client)
        {
            XpManager.GhostLevelUp += PlaceholderName;
            client.MessageReactionAdded += ReactionAdded;
            client.MessageReactionRemoved += ReactionRemoved;
        }

        private static Task ReactionAdded(DiscordClient sender, MessageReactionAddEventArgs args)
        {
            if (args.User.IsBot) return Task.CompletedTask;
            
            Task.Run(() =>
            {
                try
                {
                    using var context = new Context();
                    context.reactionRoles.Where(r => 
                            r.channelId == args.Channel.Id && 
                            r.messageId == args.Message.Id &&
                            r.emoteId == args.Emoji.Id)
                        .ForEachAsync(async r =>
                        {
                            var role = args.Guild.GetRole(r.roleId);
                            var member = await args.Guild.GetMemberAsync(args.User.Id);
                            await member.GrantRoleAsync(role, "reaction role");
                        });
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            });

            return Task.CompletedTask;
        }

        private static Task ReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs args)
        {
            if (args.User.IsBot) return Task.CompletedTask;
            
            Task.Run(() =>
            {
                try
                {
                    using var context = new Context();
                    context.reactionRoles
                        .Where(r => 
                            r.channelId == args.Channel.Id && 
                            r.messageId == args.Message.Id &&
                            r.emoteId == args.Emoji.Id)
                        .ForEachAsync(async r =>
                        {
                            var role = args.Guild.GetRole(r.roleId);
                            var member = await args.Guild.GetMemberAsync(args.User.Id);
                            await member.RevokeRoleAsync(role, "reaction role");
                        });
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            });

            return Task.CompletedTask;
        }

        public void Cleanup()
        {
            //throw new System.NotImplementedException();
        }

        private static void PlaceholderName(object sender, XpEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    var lvlArgs = (LevelUpArgs) args;
                    using var context = new Context();
                    context.xpLevelRoles.Where(r => r.lvl == lvlArgs.oldLvl).ForEachAsync(r =>
                    {
                        if (r.remain) return;
                        var role = args.context.Guild.GetRole(r.roleId);
                        args.context.Guild.GetMemberAsync(args.id).ContinueWith(t => t.Result.RevokeRoleAsync(role));
                    });
                    context.xpLevelRoles.Where(r => r.lvl == lvlArgs.lvl).ForEachAsync(r =>
                    {
                        var role = args.context.Guild.GetRole(r.roleId);
                        args.context.Guild.GetMemberAsync(args.id).ContinueWith(t => t.Result.GrantRoleAsync(role));
                    });
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            });
        }
    }
}