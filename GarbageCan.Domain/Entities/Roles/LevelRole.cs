namespace GarbageCan.Domain.Entities.Roles
{
    public class LevelRole
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong RoleId { get; set; }
        public int Lvl { get; set; }
        public bool Remain { get; set; }
    }
}
