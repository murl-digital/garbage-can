using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Serilog;

namespace GarbageCan.XP
{
    public class XpEvents : IFeature
    {
        public void Init(DiscordClient client)
        {
            XpManager.LevelUp += OnLevelUp;
        }

        public void Cleanup()
        {
            //nope
        }

        private static void OnLevelUp(object sender, LevelUpArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    var member = await e.context.Guild.GetMemberAsync(e.id);
                    var webhook = await e.context.CreateWebhookAsync(member.DisplayName);

                    var data = new DiscordWebhookBuilder()
                        .WithAvatarUrl(member.AvatarUrl)
                        .AddEmbed(
                            new DiscordEmbedBuilder()
                                .WithColor(new DiscordColor(204, 255, 94))
                                .WithTitle("Level up!")
                                .WithDescription($"Congrats to {member.DisplayName} for reaching level {e.lvl}!")
                        );

                    await webhook.ExecuteAsync(data);
                    await webhook.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            });
        }
    }
}