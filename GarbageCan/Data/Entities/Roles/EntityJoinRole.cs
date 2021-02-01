using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Data.Entities.Roles
{
    public class EntityJoinRole
    {
        [Key] public int id { get; set; }
        public ulong roleId { get; set; }
    }
}