using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Data;
using GarbageCan.Data.Entities.Moderation;
using GarbageCan.Data.Models.Moderation;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace GarbageCan.Moderation
{
    public class ModManager : IFeature
    {
        private static DiscordRole _mutedRole;
        private static readonly Timer Timer = new(TimeSpan.FromMinutes(1).TotalMilliseconds);

        private static ulong MutedRoleId
        {
            get
            {
                using var context = new Context();
                return ulong.Parse(context.config.First(c => c.key == "mutedRoleId").value);
            }
        }

        public void Init(DiscordClient client)
        {
            client.GuildDownloadCompleted += (sender, _) =>
            {
                _mutedRole = sender.Guilds[GarbageCan.OperatingGuildId].GetRole(MutedRoleId);
                Timer.Enabled = true;
                return Task.CompletedTask;
            };

            Timer.Elapsed += Tick;
        }

        public void Cleanup()
        {
        }

        public static void Log(ulong uId, ulong mId, PunishmentLevel level, string comments, out ActionLog? logEntry)
        {
            try
            {
                using var context = new Context();
                var now = DateTime.Now.ToUniversalTime();
                var log = new EntityActionLog
                {
                    uId = uId,
                    mId = mId,
                    issuedDate = now,
                    punishmentLevel = level,
                    comments = comments
                };
                context.moderationActionLogs.Add(log);

                context.SaveChanges();

                logEntry = new ActionLog
                {
                    id = log.id,
                    uId = uId,
                    mId = mId,
                    issuedDate = now,
                    punishmentLevel = level,
                    comments = comments
                };
            }
            catch (Exception e)
            {
                Serilog.Log.Error(e, "Couldn't log action");
                logEntry = null;
            }
        }

        public static void Log(ulong uId, ulong mId, PunishmentLevel level, string comments)
        {
            try
            {
                using var context = new Context();
                var now = DateTime.Now.ToUniversalTime();
                var log = new EntityActionLog
                {
                    uId = uId,
                    mId = mId,
                    issuedDate = now,
                    punishmentLevel = level,
                    comments = comments
                };
                context.moderationActionLogs.Add(log);

                context.SaveChanges();
            }
            catch (Exception e)
            {
                Serilog.Log.Error(e, "Couldn't log action");
            }
        }

        public static void Mute(DiscordMember member, DiscordMember moderator, TimeSpan span, string comments)
        {
            Task.Run(async () =>
            {
                try
                {
                    await member.GrantRoleAsync(_mutedRole, "user muted for " + span.Humanize());

                    await using var context = new Context();
                    context.moderationActiveMutes.Add(new EntityActiveMute
                    {
                        uId = member.Id,
                        expirationDate = DateTime.Now.ToUniversalTime().Add(span)
                    });

                    ModUtil.SendMessage(member.Id, $"You have been muted for {span.Humanize()}." +
                                                   $"\n\nAdditional comments: {comments}");

                    await context.SaveChangesAsync();

                    Log(member.Id, moderator.Id, PunishmentLevel.Mute,
                        $"Muted for {span.Humanize()}. Additional comments: {comments}");
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e, "Couldn't mute member");
                }
            });
        }

        public static void RestrictChannel(DiscordMember member, DiscordMember moderator, DiscordChannel channel,
            TimeSpan span, string comments)
        {
            Task.Run(async () =>
            {
                try
                {
                    await channel.AddOverwriteAsync(member, Permissions.None, Permissions.AccessChannels);

                    using var context = new Context();
                    context.moderationActiveChannelRestricts.Add(new EntityActiveChannelRestrict
                    {
                        uId = member.Id,
                        channelId = channel.Id,
                        expirationDate = DateTime.Now.ToUniversalTime().Add(span)
                    });

                    ModUtil.SendMessage(member.Id,
                        $"Your access to the {channel.Name} channel has been restricted for {span.Humanize()}. " +
                        $"\n\nAdditional comments: {comments}");

                    await context.SaveChangesAsync();

                    Log(member.Id, moderator.Id, PunishmentLevel.ChannelRestrict,
                        $"Restricted access to {channel.Name} for {span.Humanize()}. Additional comments: {comments}");
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e, "Couldn't restrict channel");
                }
            });
        }

        private static void Tick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now.ToUniversalTime();
                    using var context = new Context();

                    await context.moderationActiveMutes
                        .Where(m => m.expirationDate <= now)
                        .ForEachAsync(async m =>
                        {
                            var member = await GarbageCan.Client.Guilds[GarbageCan.OperatingGuildId]
                                .GetMemberAsync(m.uId);
                            await member.RevokeRoleAsync(_mutedRole, "mute expired");
                        });

                    await context.moderationActiveMutes
                        .Where(m => m.expirationDate <= now)
                        .DeleteAsync();

                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Serilog.Log.Information(e, "Couldn't unmute user");
                }
            });

            Task.Run(async () =>
            {
                try
                {
                    var now = DateTime.Now.ToUniversalTime();
                    using var context = new Context();

                    await context.moderationActiveChannelRestricts
                        .Where(c => c.expirationDate <= now)
                        .ForEachAsync(async c =>
                        {
                            var member = await GarbageCan.Client.Guilds[GarbageCan.OperatingGuildId]
                                .GetMemberAsync(c.uId);
                            var channel = GarbageCan.Client.Guilds[GarbageCan.OperatingGuildId]
                                .GetChannel(c.channelId);

                            await channel.PermissionOverwrites.First(o => o.Id == member.Id)
                                .DeleteAsync("channel restrict expired");
                        });

                    await context.moderationActiveChannelRestricts
                        .Where(c => c.expirationDate <= now)
                        .DeleteAsync();

                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e, "Couldn't un-restrict channel");
                }
            });
        }
    }
}