using GarbageCan.Application.Common.Configuration;

namespace GarbageCan.Web.Configurations
{
    internal class RoleConfiguration : IRoleConfiguration
    {
        public ulong MuteRoleId { get; set; }
    }
}