using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using GarbageCan.XP.Boosters;
using GarbageCan.XP.Data;
using GarbageCan.XP.Data.Entities;
using MathNet.Numerics.Distributions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GarbageCan.XP
{
    public class XpManager : IFeature
    {
        private Normal _random;
        
        //LevelUp is fired once every time a user levels up (i.e from level 1 to 2, or from 5 to 10 (which can happen)
        //GhostLevelUp is fired every time a user's level increments (i.e from level 1 to 2 = 1 invoke, from level 5 to 10 5 invokes)
        public static event EventHandler<LevelUpArgs> LevelUp;
        public static event EventHandler<XpEventArgs> GhostLevelUp;
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

        private Task HandleJoin(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            if (e.Member.IsBot) 
                return Task.CompletedTask;

            Task.Run(async () =>
            {
                await using var context = new XpContext();
                var user = new EntityUser
                {
                    id = e.Member.Id,
                    lvl = 0,
                    xp = 0
                };

                await context.xpUsers.AddAsync(user);
                await context.SaveChangesAsync();
            });
            
            return Task.CompletedTask;
        }
        
        private Task HandleMessage(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Channel.IsPrivate || e.Author.IsBot || e.Author.IsSystem.HasValue && e.Author.IsSystem.Value || e.Message.Content.StartsWith(GarbageCan.Config.commandPrefix) || IsExcluded(e.Channel.Id))
                return Task.CompletedTask;

            Task.Run(async () =>
            {
                await using var context = new XpContext();
                var user = await context.xpUsers
                    .Where(u => u.id == e.Author.Id)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    user = new EntityUser
                    {
                        id = e.Author.Id,
                        lvl = 0,
                        xp = 0
                    };

                    await context.xpUsers.AddAsync(user);
                }
                
                user.xp += XpEarned(e.Message.Content);

                var oldLevel = user.lvl;
                while (user.xp > TotalXpRequired(user.lvl))
                {
                    Log.Information(TotalXpRequired(user.lvl).ToString());
                    user.lvl++;
                    GhostLevelUp?.Invoke(this, new LevelUpArgs
                    {
                        client = sender,
                        context = e,
                        id = e.Author.Id,
                        lvl = user.lvl,
                        oldLvl = oldLevel,
                        xp = user.xp   
                    });
                }

                if (user.lvl > oldLevel)
                {
                    LevelUp.Invoke(this, new LevelUpArgs
                    {
                        client = sender,
                        context = e,
                        id = e.Author.Id,
                        lvl = user.lvl,
                        oldLvl = oldLevel,
                        xp = user.xp
                    });
                }

                await context.SaveChangesAsync();
            });
            
            return Task.CompletedTask;
        }

        private bool IsExcluded(ulong channelId)
        {
            using var context = new XpContext();
            return context.xpExcludedChannels
                .Any(c => c.channel_id == channelId);
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
            for (var i = 0; i <= lvl; i++)
            {
                result += XpRequired(i);
            }

            return result;
        }
    }

    public class XpEventArgs : EventArgs
    {
        public DiscordClient client { get; set; }
        public MessageCreateEventArgs context { get; set; }
        public ulong id { get; set; }
        public int lvl { get; set; }

        private double _xp;
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