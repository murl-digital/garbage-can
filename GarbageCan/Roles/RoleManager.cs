using System;
using System.Collections.Generic;
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
        private readonly List<ulong> _watchedUsers = new();
        public void Init(DiscordClient client)
        {
            XpManager.GhostLevelUp += HandleLevelRoles;
            client.MessageReactionAdded += ReactionAdded;
            client.MessageReactionRemoved += ReactionRemoved;

            client.GuildMemberAdded += (_, args) =>
            {
                _watchedUsers.Add(args.Member.Id);
                return Task.CompletedTask;
            }; 
            client.GuildMemberUpdated += HandleJoinRoles;
            client.GuildMemberUpdated += HandleConditionalRoles;
        }

        private static Task HandleConditionalRoles(DiscordClient sender, GuildMemberUpdateEventArgs e)
        {
            if (e.Member.IsBot) return Task.CompletedTask;
            if (e.RolesBefore.Count == e.RolesAfter.Count) return Task.CompletedTask;

            Task.Run(async () =>
            {
                try
                {
                    await using var context = new Context();

                    if (e.RolesBefore.Count < e.RolesAfter.Count)
                    {
                        var roles = e.RolesAfter.Except(e.RolesBefore);

                        foreach (var role in roles)
                        {
                            if (!context.conditionalRoles.Any(r => r.requiredRoleId == role.Id)) continue;

                            await context.conditionalRoles.Where(r => r.requiredRoleId == role.Id).ForEachAsync(r =>
                            {
                                var toBeAssigned = e.Guild.GetRole(r.resultRoleId);
                                e.Member.GrantRoleAsync(toBeAssigned);
                            });
                        }
                    }
                
                    if (e.RolesBefore.Count > e.RolesAfter.Count)
                    {
                        var roles = e.RolesBefore.Except(e.RolesAfter);

                        foreach (var role in roles)
                        {
                            if (!context.conditionalRoles.Any(r => r.requiredRoleId == role.Id)) continue;
                            if (context.conditionalRoles.Count(r =>
                                r.resultRoleId == role.Id && e.RolesAfter.Select(d => d.Id).Contains(r.requiredRoleId)) > 1)
                                continue;

                            await context.conditionalRoles.Where(r => r.requiredRoleId == role.Id && !r.remain).ForEachAsync(r =>
                            {
                                var toBeRemoved = e.Guild.GetRole(r.resultRoleId);
                                e.Member.RevokeRoleAsync(toBeRemoved);
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "uh oh");
                }
            });
            
            return Task.CompletedTask;
        }

        private Task HandleJoinRoles(DiscordClient sender, GuildMemberUpdateEventArgs e)
        {
            try
            {
                if (e.Member.IsBot) return Task.CompletedTask;
                if (!_watchedUsers.Contains(e.Member.Id)) return Task.CompletedTask;
                if (e.Member.IsPending ?? true) return Task.CompletedTask;
                _watchedUsers.Remove(e.Member.Id);

                Task.Run(async () =>
                {
                    await using var context = new Context();

                    await context.joinRoles.ForEachAsync(async r =>
                    {
                        try
                        {
                            var role = e.Guild.GetRole(r.roleId);
                            await e.Member.GrantRoleAsync(role, "join role");
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "couldn't grant role to user");
                        }
                    });
                });

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong");
                return Task.CompletedTask;
            }
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
            Task.Run(async () =>
            {
                try
                {
                    var lvlArgs = (LevelUpArgs) args;
                    await using var context = new Context();
                    await context.levelRoles.Where(r => r.lvl == lvlArgs.oldLvl && !r.remain).ForEachAsync(async r =>
                    {
                        var role = args.context.Guild.GetRole(r.roleId);
                        var member = await args.context.Guild.GetMemberAsync(args.id);
                        await member.RevokeRoleAsync(role);
                    });
                    await context.levelRoles.Where(r => r.lvl == lvlArgs.lvl).ForEachAsync(async r =>
                    {
                        var role = args.context.Guild.GetRole(r.roleId);
                        var member = await args.context.Guild.GetMemberAsync(args.id);
                        await member.GrantRoleAsync(role);
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