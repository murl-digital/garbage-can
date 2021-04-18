using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Domain.Entities.Roles
{
    public class EntityJoinRole
    {
        [Key] public int id { get; set; }
        public ulong roleId { get; set; }
    }
}