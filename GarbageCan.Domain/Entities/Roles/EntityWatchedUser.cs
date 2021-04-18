using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Domain.Entities.Roles
{
    public class EntityWatchedUser
    {
        [Key] public ulong id { get; set; }
    }
}