using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
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
            XpManager.GhostLevelUp += HandleLevelRoles;
            client.MessageReactionAdded += ReactionAdded;
            client.MessageReactionRemoved += ReactionRemoved;
        }

        #region reaction roles

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
                            r.emoteId == EmoteId(args.Emoji))
                        .ForEachAsync(async r =>
                        {
                            try
                            {
                                var role = args.Guild.GetRole(r.roleId);
                                var member = await args.Guild.GetMemberAsync(args.User.Id);
                                await member.GrantRoleAsync(role, "reaction role");
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Couldn't assign reaction role"); 
                            }
                        });
                }
                catch (Exception e)
                {
                    Log.Error(e, "uh oh");
                }
            });

            return Task.CompletedTask;
        }

        private static Task ReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs args)
        {
            if (args.User.IsBot) return Task.CompletedTask;
            
            Task.Run(async () =>
            {
                try
                {
                    using var context = new Context();
                    await context.reactionRoles
                        .Where(r => 
                            r.channelId == args.Channel.Id && 
                            r.messageId == args.Message.Id &&
                            r.emoteId == EmoteId(args.Emoji))
                        .ForEachAsync(async r =>
                        {
                            try
                            {
                                var role = args.Guild.GetRole(r.roleId);
                                var member = await args.Guild.GetMemberAsync(args.User.Id);
                                await member.RevokeRoleAsync(role, "reaction role");
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Couldn't remove reaction role");
                            }
                        });
                }
                catch (Exception e)
                {
                    Log.Error(e, "uh oh");
                }
            });

            return Task.CompletedTask;
        }

        #endregion

        public void Cleanup()
        {
        }

        #region level roles

        private static void HandleLevelRoles(object sender, XpEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    var lvlArgs = (LevelUpArgs) args;
                    using var context = new Context();
                    context.levelRoles.Where(r => r.lvl == lvlArgs.oldLvl).ForEachAsync(r =>
                    {
                        if (r.remain) return;
                        var role = args.context.Guild.GetRole(r.roleId);
                        args.context.Guild.GetMemberAsync(args.id).ContinueWith(t => t.Result.RevokeRoleAsync(role));
                    });
                    context.levelRoles.Where(r => r.lvl == lvlArgs.lvl).ForEachAsync(r =>
                    {
                        var role = args.context.Guild.GetRole(r.roleId);
                        args.context.Guild.GetMemberAsync(args.id).ContinueWith(t => t.Result.GrantRoleAsync(role));
                    });
                }
                catch (Exception e)
                {
                    Log.Error(e, "Level roles failed");
                }
            });
        }

        #endregion

        public static string EmoteId(DiscordEmoji emote) => emote.Id == 0 ? emote.Name : emote.Id.ToString();

    }
}