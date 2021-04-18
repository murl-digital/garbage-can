namespace GarbageCan.Domain.Entities.Roles
{
    public class EntityLevelRole
    {
        public int id { get; set; }
        public int lvl { get; set; }
        public ulong roleId { get; set; }
        public bool remain { get; set; }
    }
}