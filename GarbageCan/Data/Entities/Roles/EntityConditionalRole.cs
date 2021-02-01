using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities.Roles
{
    public class EntityConditionalRole
    {
        [Key] public int id { get; set; }
        public ulong requiredRoleId { get; set; }
        public ulong resultRoleId { get; set; }
        public bool remain { get; set; }
    }
}