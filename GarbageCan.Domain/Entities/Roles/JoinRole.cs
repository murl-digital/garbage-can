namespace GarbageCan.Domain.Entities.Roles
{
    public class JoinRole
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong RoleId { get; set; }
    }
}
