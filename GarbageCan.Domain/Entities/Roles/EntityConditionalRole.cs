namespace GarbageCan.Domain.Entities.Roles
{
    public class EntityConditionalRole
    {
        public int id { get; set; }
        public ulong requiredRoleId { get; set; }
        public ulong resultRoleId { get; set; }
        public bool remain { get; set; }
    }
}