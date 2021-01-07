using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using GarbageCan.Data;
using GarbageCan.Data.Entities;
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
        public static void LogWarn(ulong uId, string comments)
        {
            using var context = new Context();
            var now = DateTime.Now.ToUniversalTime();
            context.moderationWarnings.Add(new EntityWarning
            {
                uId = uId,
                issuedDate = now,
                comments = comments
            });
        }

        public static void Mute(ulong uId, TimeSpan span)
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
                
                ModUtil.SendMessage(uId, "You have been muted for " + span.Humanize());
            });
        }

        public static void RestrictChannel(ulong uId, ulong channelId, TimeSpan span)
        {
            Task.Run(async () =>
            {
                var member = await GetMember(uId);
                var channel = GarbageCan.Client.Guilds[0].GetChannel(channelId);

                await channel.AddOverwriteAsync(member, Permissions.None, Permissions.AccessChannels);

                using var context = new Context();
                context.moderationActiveChannelRestricts.Add(new EntityActiveChannelRestrict
                {
                    uId = uId,
                    channelId = channelId,
                    expirationDate = DateTime.Now.ToUniversalTime().Add(span)
                });
                
                ModUtil.SendMessage(uId, "Your access to the " + channel.Name + " has been restricted for " + span.Humanize());
            });
        }

        private static async Task<DiscordMember> GetMember(ulong uId)
        {
            var member = await GarbageCan.Client.Guilds[0].GetMemberAsync(uId);
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
                        var member = await GarbageCan.Client.Guilds[0].GetMemberAsync(m.uId);
                        await member.RevokeRoleAsync(_mutedRole, "mute expired");
                    });
                
                await context.moderationActiveMutes
                    .Where(m => m.expirationDate <= now)
                    .DeleteAsync();
            });
            
            Task.Run(async () =>
            {
                var now = DateTime.Now.ToUniversalTime();
                using var context = new Context();
                
                await context.moderationActiveChannelRestricts
                    .Where(c => c.expirationDate <= now)
                    .ForEachAsync(async c =>
                    {
                        var member = await GarbageCan.Client.Guilds[0].GetMemberAsync(c.uId);
                        var channel = GarbageCan.Client.Guilds[0].GetChannel(c.channelId);

                        await channel.AddOverwriteAsync(member, Permissions.AccessChannels);
                    });
                
                await context.moderationActiveChannelRestricts
                    .Where(c => c.expirationDate <= now)
                    .DeleteAsync();
            });
        }

        public void Init(DiscordClient client)
        {
            client.Ready += (sender, _) =>
            {
                _mutedRole = sender.Guilds[0].GetRole(GarbageCan.Config.mutedRoleId);
                Timer.Enabled = true;
                return Task.CompletedTask;
            };

            Timer.Elapsed += Tick;
        }

        public void Cleanup()
        {
            throw new NotImplementedException();
        }
    }
}