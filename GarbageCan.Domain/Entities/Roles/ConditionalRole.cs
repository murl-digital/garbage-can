namespace GarbageCan.Domain.Entities.Roles
{
    public class ConditionalRole
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong RequiredRoleId { get; set; }
        public ulong ResultRoleId { get; set; }
        public bool Remain { get; set; }
    }
}
