using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace GarbageCan
{
    public class AprilFools : IFeature
    {
        private static readonly Random Random = new();
        private static readonly Timer BanTimer = new(TimeSpan.FromHours(1).TotalMilliseconds);

        private static readonly string[] StupidReplies = {
            "pawg",
            "shut up",
            "FUCKING CUM",
            "this made my grandma fall over",
            "ew",
            "cbt!",
            "*innit bruv in russian*",
            "what a bagner",
            "sad",
            "THIS IS SO TRUE",
            "bc fuck you",
            ":(",
            "yike",
            "oof",
            "BUT MY PUSSY IS sorry-",
            "cun, spern, even",
            "Yep!",
            "Shut the fuck up xoxo"
        };
        public void Init(DiscordClient client)
        {
            BanTimer.Elapsed += FakeBan;
            client.MessageCreated += ClientOnMessageCreated;
            client.Ready += (_, _) =>
            {
                Task.Run(async () =>
                {
                    BanTimer.Enabled = true;
                });

                return Task.CompletedTask;
            };
        }

        private Task ClientOnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Author.IsBot) return Task.CompletedTask;
            
            Task.Run(async () =>
            {
                if (Random.Next(2) == 1)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Random.Next(1, 120)));
                    
                    if (Random.Next(11) > 2)
                    {
                        var builder = new DiscordMessageBuilder().WithContent(e.Message.Content).WithReply(e.Message.Id, true);
                        await e.Channel.SendMessageAsync(builder);
                    }
                    else
                    {
                        var builder = new DiscordMessageBuilder().WithContent(StupidReplies.ElementAt(Random.Next(StupidReplies.Length))).WithReply(e.Message.Id);
                        await e.Channel.SendMessageAsync(builder);
                    }
                }
            });
            
            return Task.CompletedTask;
        }

        private void FakeBan(object sender, ElapsedEventArgs e)
        {
            Task.Run(async () =>
            {
                var guild = await GarbageCan.Client.GetGuildAsync(GarbageCan.OperatingGuildId);
                var members = await guild.GetAllMembersAsync();
                var membersWithRole = members.Where(m => m.Roles.Any(r => r.Id == 791723521968570429)).ToList();
                var member = membersWithRole.ElementAt(Random.Next(membersWithRole.Count));
                var role = guild.GetRole(791723521968570429);

                await member.RevokeRoleAsync(role);
                await guild.GetChannel(791718260239499266).SendMessageAsync($"{member.Mention} has been baned for [REASON NOT SPECFIED]");
            });
        }



        public void Cleanup()
        {
            
        }
    }
}