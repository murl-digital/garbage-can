using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using GarbageCan.Data;
using GarbageCan.XP;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GarbageCan.Roles
{
    public class RoleManager : IFeature
    {
        public void Init(DiscordClient client)
        {
            XpManager.GhostLevelUp += PlaceholderName;
        }

        public void Cleanup()
        {
            //throw new System.NotImplementedException();
        }

        private static void PlaceholderName(object sender, XpEventArgs args)
        {
            Task.Run(() =>
            {
                try
                {
                    var lvlArgs = (LevelUpArgs) args;
                    using var context = new Context();
                    context.xpLevelRoles.Where(r => r.lvl == lvlArgs.oldLvl).ForEachAsync(r =>
                    {
                        if (r.remain) return;
                        var role = args.context.Guild.GetRole(r.roleId);
                        args.context.Guild.GetMemberAsync(args.id).ContinueWith(t => t.Result.RevokeRoleAsync(role));
                    });
                    context.xpLevelRoles.Where(r => r.lvl == lvlArgs.lvl).ForEachAsync(r =>
                    {
                        var role = args.context.Guild.GetRole(r.roleId);
                        args.context.Guild.GetMemberAsync(args.id).ContinueWith(t => t.Result.GrantRoleAsync(role));
                    });
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            });
        }
    }
}