using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
using DSharpPlus.Entities;
using GarbageCan.Data;
using GarbageCan.Data.Entities.Moderation;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace GarbageCan.Moderation
{
    public class ModManager : IFeature
    {
        private static DiscordRole _mutedRole;
        private static readonly Timer Timer = new(TimeSpan.FromMinutes(1).TotalMilliseconds);

        public void Init(DiscordClient client)
        {
            client.Ready += (sender, _) =>
            {
                _mutedRole = sender.Guilds[GarbageCan.Config.operatingGuildId].GetRole(GarbageCan.Config.mutedRoleId);
                Timer.Enabled = true;
                return Task.CompletedTask;
            };

            Timer.Elapsed += Tick;
        }

        public void Cleanup()
        {
            throw new NotImplementedException();
        }

        public static void Log(ulong uId, PunishmentLevel level, string comments)
        {
            Task.Run(async () =>
            {
                using var context = new Context();
                var now = DateTime.Now.ToUniversalTime();
                context.moderationActionLogs.Add(new EntityActionLog
                {
                    uId = uId,
                    issuedDate = now,
                    punishmentLevel = level,
                    comments = comments
                });

                await context.SaveChangesAsync();
            });
        }

        public static void Mute(ulong uId, TimeSpan span, string comments)
        {
            Task.Run(async () =>
            {
                var member = await GetMember(uId);

                await member.GrantRoleAsync(_mutedRole, "user muted for " + span.Humanize());

                await using var context = new Context();
                context.moderationActiveMutes.Add(new EntityActiveMute
                {
                    uId = uId,
                    expirationDate = DateTime.Now.ToUniversalTime().Add(span)
                });

                ModUtil.SendMessage(uId, $"You have been muted for {span.Humanize()}." +
                                         $"\n\nAdditional comments: {comments}");
                
                await context.SaveChangesAsync();
                
                Log(uId, PunishmentLevel.Mute, $"Muted for {span.Humanize()}. Additional comments: {comments}");
            });
        }

        public static void RestrictChannel(ulong uId, ulong channelId, TimeSpan span, string comments)
        {
            Task.Run(async () =>
            {
                var member = await GetMember(uId);
                var channel = GarbageCan.Client.Guilds[GarbageCan.Config.operatingGuildId].GetChannel(channelId);

                await channel.AddOverwriteAsync(member, Permissions.None, Permissions.AccessChannels);

                using var context = new Context();
                context.moderationActiveChannelRestricts.Add(new EntityActiveChannelRestrict
                {
                    uId = uId,
                    channelId = channelId,
                    expirationDate = DateTime.Now.ToUniversalTime().Add(span)
                });

                ModUtil.SendMessage(uId, 
                    $"Your access to the {channel.Name} channel has been restricted for {span.Humanize()}. " +
                    $"\n\nAdditional comments: {comments}");

                await context.SaveChangesAsync();
                
                Log(uId, PunishmentLevel.ChannelRestrict, $"Restricted access to {channel.Name} for {span.Humanize()}. Additional comments: {comments}");
            });
        }

        private static async Task<DiscordMember> GetMember(ulong uId)
        {
            var member = await GarbageCan.Client.Guilds[GarbageCan.Config.operatingGuildId].GetMemberAsync(uId);
            return member;
        }

        private static void Tick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Task.Run(async () =>
            {
                var now = DateTime.Now.ToUniversalTime();
                using var context = new Context();

                await context.moderationActiveMutes
                    .Where(m => m.expirationDate <= now)
                    .ForEachAsync(async m =>
                    {
                        var member = await GarbageCan.Client.Guilds[GarbageCan.Config.operatingGuildId].GetMemberAsync(m.uId);
                        await member.RevokeRoleAsync(_mutedRole, "mute expired");
                    });

                await context.moderationActiveMutes
                    .Where(m => m.expirationDate <= now)
                    .DeleteAsync();
                
                await context.SaveChangesAsync();
            });

            Task.Run(async () =>
            {
                var now = DateTime.Now.ToUniversalTime();
                using var context = new Context();

                await context.moderationActiveChannelRestricts
                    .Where(c => c.expirationDate <= now)
                    .ForEachAsync(async c =>
                    {
                        var member = await GarbageCan.Client.Guilds[GarbageCan.Config.operatingGuildId].GetMemberAsync(c.uId);
                        var channel = GarbageCan.Client.Guilds[GarbageCan.Config.operatingGuildId].GetChannel(c.channelId);

                        await channel.AddOverwriteAsync(member, Permissions.AccessChannels);
                    });

                await context.moderationActiveChannelRestricts
                    .Where(c => c.expirationDate <= now)
                    .DeleteAsync();
                
                await context.SaveChangesAsync();
            });
        }
    }
}