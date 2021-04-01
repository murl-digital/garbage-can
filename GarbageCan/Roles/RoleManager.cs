using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GarbageCan.Data;
using GarbageCan.Data.Entities.Roles;
using GarbageCan.XP;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Z.EntityFramework.Plus;

namespace GarbageCan.Roles
{
    public class RoleManager : IFeature
    {
        private readonly Timer _roleTaskTimer = new(5000);

        public void Init(DiscordClient client)
        {
            XpManager.GhostLevelUp += HandleLevelRoles;
            client.MessageReactionAdded += ReactionAdded;
            client.MessageReactionRemoved += ReactionRemoved;

            client.GuildMemberAdded += (_, args) =>
            {
                using var context = new Context();
                context.joinWatchlist.Add(new EntityWatchedUser
                {
                    id = args.Member.Id
                });
                context.SaveChanges();
                return Task.CompletedTask;
            };
            client.GuildMemberUpdated += HandleJoinRoles;

            client.Ready += (_, _) =>
            {
                _roleTaskTimer.Elapsed += Tick;
                _roleTaskTimer.Enabled = true;

                return Task.CompletedTask;
            };
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
                    await using var context = new Context();
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
                    await using var context = new Context();
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

        private static Task HandleJoinRoles(DiscordClient sender, GuildMemberUpdateEventArgs e)
        {
            try
            {
                if (e.Member.IsBot) return Task.CompletedTask;

                Task.Run(async () =>
                {
                    await using var context = new Context();

                    if (context.joinWatchlist.Any(u => u.id == e.Member.Id))
                    {
                        await context.joinWatchlist.Where(u => u.id == e.Member.Id).DeleteAsync();

                        await context.joinRoles.ForEachAsync(async r =>
                        {
                            try
                            {
                                var role = e.Guild.GetRole(r.roleId);
                                await e.Member.GrantRoleAsync(role, "join role");
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "couldn't grant role to user");
                            }
                        });
                    }
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
                    for (var i = 0; i < roles.Length - 1; i++)
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

        private static void Tick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Task.Run(async () =>
            {
                try
                {
                    await using var context = new Context();
                    var conditionalRoles = new Dictionary<ulong, List<ulong>>();
                    var conditionalRolesEntity = await context.conditionalRoles.ToArrayAsync();
                    var members = await GarbageCan.Client.Guilds[GarbageCan.OperatingGuildId].GetAllMembersAsync();
                    var taskList = new List<Task>();

                    foreach (var role in conditionalRolesEntity)
                    {
                        if (!conditionalRoles.TryGetValue(role.resultRoleId, out var requiredRoles))
                        {
                            requiredRoles = new List<ulong>();
                            conditionalRoles.Add(role.resultRoleId, requiredRoles);
                        }

                        requiredRoles.Add(role.requiredRoleId);
                    }

                    var keySet = conditionalRoles.Keys;

                    foreach (var member in members)
                    {
                        foreach (var role in member.Roles)
                        {
                            if (keySet.All(k => k != role.Id)) continue;
                            if (!member.Roles.Select(r => r.Id).Any(r => conditionalRoles[role.Id].Any(i => i == r)))
                            {
                                if (conditionalRolesEntity.First(r => r.resultRoleId == role.Id).remain) continue;
                                taskList.Add(member.RevokeRoleAsync(role, "conditional roles"));
                            }
                        }

                        var roles = member.Roles.Select(r => r.Id).ToArray();
                        foreach (var (key, value) in conditionalRoles)
                        {
                            if (roles.Any(r => value.Any(i => i == r)) && roles.All(r => r != key))
                            {
                                taskList.Add(member.GrantRoleAsync(member.Guild.GetRole(key), "conditional roles"));
                            }
                        }
                    }

                    await Task.WhenAll(taskList);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Conditional role update failed");
                }
            });
        }
    }
}