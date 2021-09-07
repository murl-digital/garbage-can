using GarbageCan.Application.Common.Configuration;

namespace GarbageCan.Configurations
{
    internal class RoleConfiguration : IRoleConfiguration
    {
        public ulong MuteRoleId { get; set; }
    }
}