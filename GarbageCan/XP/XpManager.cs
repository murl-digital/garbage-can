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
        public void Init(DiscordClient client)
        {
            client.MessageCreated += HandleMessage;
            
            _random = new Normal(0, 1);
        }

        public void Cleanup()
        {
            //nope
        }
        
        private Task HandleMessage(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Channel.IsPrivate)
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

                while (user.xp > TotalXpRequired(user.lvl))
                {
                    Log.Information(TotalXpRequired(user.lvl).ToString());
                    user.lvl++;
                }

                await context.SaveChangesAsync();
            });
            
            return Task.CompletedTask;
        }

        private double XpEarned(string message)
        {
            var length = Math.Sqrt(message.Replace(" ", "").Length);
            length = Math.Min(10, length);

            return length * (Math.Abs(_random.Sample()) * 5 + 1) * BoosterManager.GetMultiplier();
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
}