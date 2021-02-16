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

        public void Cleanup()
        {
        }
        
        public static string EmoteId(DiscordEmoji emote) => emote.Id == 0 ? emote.Name : emote.Id.ToString();

        private static Task ReactionAdded(DiscordClient sender, MessageReactionAddEventArgs args)
        {
            if (args.User.IsBot) return Task.CompletedTask;

            Task.Run(async () =>
            {
                try
                {
                    using var context = new Context();
                    await context.reactionRoles.ForEachAsync(async r =>
                        {
                            try
                            {
                                // before you say "BUT JOE YOU CAN USE .WHERE() JOE" hear me out
                                // i tried that. i really did. but for some reason it didnt work.
                                // it would just say "hey all these rows satisfy the predicate!" ...even though they don't
                                // conclusion: linq is a lie thank you for coming to my ted talk
                                if (r.channelId != args.Channel.Id || r.messageId != args.Message.Id ||
                                    r.emoteId != EmoteId(args.Emoji)) return;
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
                    await context.reactionRoles.ForEachAsync(async r =>
                        {
                            try
                            {
                                if (r.channelId != args.Channel.Id || r.messageId != args.Message.Id ||
                                    r.emoteId != EmoteId(args.Emoji)) return;
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

        private Task HandleJoinRoles(DiscordClient sender, GuildMemberUpdateEventArgs e)
        {
            try
            {
                if (e.Member.IsBot) return Task.CompletedTask;
                if (!_watchedUsers.Contains(e.Member.Id)) return Task.CompletedTask;
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

        private static void HandleLevelRoles(object sender, XpEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var tasks = new List<Task>();
                    
                    var lvlArgs = (LevelUpArgs) args;
                    var member = await args.context.Guild.GetMemberAsync(args.id);
                    var memberRoles = member.Roles.Select(r => r.Id).ToArray();
                    
                    await using var context = new Context();
                    var roles = context.levelRoles.OrderBy(r => r.lvl).Where(r => !r.remain).ToArray();
                    for (var i = 0; i < roles.Length-1; i++)
                    {
                        if (roles[i].lvl > lvlArgs.lvl) break;
                        if (!memberRoles.Contains(roles[i].roleId)) continue;
                        if (lvlArgs.lvl >= roles[i].lvl && lvlArgs.lvl < roles[i + 1].lvl) continue;
                        
                        var role = member.Guild.GetRole(roles[i].roleId);
                        tasks.Add(member.RevokeRoleAsync(role, "level roles"));
                    }
                    
                    tasks.Add(context.levelRoles.Where(r => r.lvl == lvlArgs.lvl).ForEachAsync(async r =>
                    {
                        var role = args.context.Guild.GetRole(r.roleId);
                        await member.GrantRoleAsync(role, "level roles");
                    }));

                    await Task.WhenAll(tasks.ToArray());
                }
                catch (Exception e)
                {
                    Log.Error(e, "Level roles failed");
                }
            });
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
                                r.resultRoleId == role.Id &&
                                e.RolesAfter.Select(d => d.Id).Contains(r.requiredRoleId)) > 1)
                                continue;

                            await context.conditionalRoles.Where(r => r.requiredRoleId == role.Id && !r.remain)
                                .ForEachAsync(r =>
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
    }
}