using System;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Serilog;

namespace GarbageCan.Moderation
{
    public static class ModUtil
    {
        public static async void SendMessage(ulong uId, string message)
        {
            var member = await GarbageCan.Client.Guilds[GarbageCan.Config.operatingGuildId].GetMemberAsync(uId);
            try
            {
                try
                {
                    var channel = await member.CreateDmChannelAsync();
                    await channel.SendMessageAsync(message);
                }
                catch (UnauthorizedException)
                {
                    var hideOverwrite = new DiscordOverwriteBuilder()
                        .Deny(Permissions.AccessChannels)
                        .For(member.Guild.EveryoneRole);
                    var showOverwrite = new DiscordOverwriteBuilder()
                        .Allow(Permissions.AccessChannels)
                        .For(member);
                    var readonlyOverwrite = new DiscordOverwriteBuilder()
                        .Deny(Permissions.SendMessages)
                        .Deny(Permissions.AddReactions)
                        .For(member);
                    var channel = await member.Guild.CreateChannelAsync(
                        "message-" + member.Username,
                        ChannelType.Text,
                        null,
                        overwrites: new [] {hideOverwrite, showOverwrite, readonlyOverwrite});

                    await channel.SendMessageAsync(
                        "Hello, " + member
                            .Mention +
                        ". We tried to send you a direct message, however your direct messages are disabled for this server. Below is the message in question.");
                    await channel.SendMessageAsync(message);
                }
            } 
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}