using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GarbageCan.Data;
using GarbageCan.Data.Entities.XP;
using GarbageCan.XP.Boosters;
using MathNet.Numerics.Distributions;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.XP
{
    public class XpManager : IFeature
    {
        private Normal _random;

        public void Init(DiscordClient client)
        {
            client.MessageCreated += HandleMessage;
            client.GuildMemberAdded += HandleJoin;

            _random = new Normal(0, 1);
        }

        public void Cleanup()
        {
            //nope
        }

        //LevelUp is fired once every time a user levels up (i.e from level 1 to 2, or from 5 to 10 (which can happen)
        //GhostLevelUp is fired every time a user's level increments (i.e from level 1 to 2 = 1 invoke, from level 5 to 10 5 invokes)
        public static event EventHandler<LevelUpArgs> LevelUp;
        public static event EventHandler<XpEventArgs> GhostLevelUp;

        private Task HandleJoin(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            if (e.Member.IsBot)
                return Task.CompletedTask;

            Task.Run(async () =>
            {
                using var context = new Context();
                var user = new EntityUser
                {
                    id = e.Member.Id,
                    lvl = 0,
                    xp = 0
                };

                context.xpUsers.Add(user);
                await context.SaveChangesAsync();
            });

            return Task.CompletedTask;
        }

        private Task HandleMessage(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Channel.IsPrivate || e.Author.IsBot || e.Author.IsSystem.HasValue && e.Author.IsSystem.Value ||
                e.Message.Content.StartsWith(GarbageCan.Config.commandPrefix) || IsExcluded(e.Channel.Id))
                return Task.CompletedTask;

            AddXp(e.Author.Id, XpEarned(e.Message.Content), sender, e);

            return Task.CompletedTask;
        }

        private void AddXp(ulong id, double amount, DiscordClient sender, MessageCreateEventArgs e)
        {
            Task.Run(async () =>
            {
                using var context = new Context();
                var user = await context.xpUsers
                    .Where(u => u.id == id)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    user = new EntityUser
                    {
                        id = id,
                        lvl = 0,
                        xp = 0
                    };

                    context.xpUsers.Add(user);
                }

                user.xp += amount;

                var oldLevel = user.lvl;
                while (user.xp > TotalXpRequired(user.lvl))
                {
                    user.lvl++;
                    GhostLevelUp?.Invoke(this, new LevelUpArgs
                    {
                        context = e.Channel,
                        id = user.id,
                        lvl = user.lvl,
                        oldLvl = oldLevel,
                        xp = user.xp
                    });
                }

                if (user.lvl > oldLevel)
                    LevelUp?.Invoke(this, new LevelUpArgs
                    {
                        context = e.Channel,
                        id = user.id,
                        lvl = user.lvl,
                        oldLvl = oldLevel,
                        xp = user.xp
                    });

                await context.SaveChangesAsync();
            });
        }

        private static bool IsExcluded(ulong channelId)
        {
            using var context = new Context();
            return context.xpExcludedChannels
                .Any(c => c.channelId == channelId);
        }

        private double XpEarned(string message)
        {
            var length = Math.Sqrt(message.Replace(" ", "").Length);
            length = Math.Min(10, length);

            var test = length * (Math.Abs(_random.Sample()) * 0.5 + 1) * BoosterManager.GetMultiplier();
            return test;
        }

        //the level argument here is a tad misleading- pass in any level as an integer to get the amount of xp a user needs to earn in order to advance to the next level
        private static double XpRequired(int lvl)
        {
            return Math.Round(250 + 75 * Math.Pow(lvl, 0.6), 1);
        }

        private static double TotalXpRequired(int lvl)
        {
            var result = 0.0;
            for (var i = 0; i <= lvl; i++) result += XpRequired(i);

            return result;
        }
    }

    public class XpEventArgs : EventArgs
    {
        private double _xp;
        public DiscordChannel context { get; set; }
        public ulong id { get; set; }
        public int lvl { get; set; }

        public double xp
        {
            get => _xp;
            set => _xp = Math.Round(value, 1);
        }
    }

    public class LevelUpArgs : XpEventArgs
    {
        public int oldLvl { get; set; }
    }
}