using System.ComponentModel.DataAnnotations;

namespace GarbageCan.Data.Entities.Roles
{
    public class EntityWatchedUser
    {
        [Key] public ulong id { get; set; }
    }
}