using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities.Roles
{
    public class EntityConditionalRole
    {
        [Key] public int id { get; set; }
        [NotMapped] public ulong[] requiredRoleIds
        {
            get => Array.ConvertAll(internalRequiredRoleIds.Split(";"), ulong.Parse);
            set => internalRequiredRoleIds = string.Join(";", value);
        }
        public string internalRequiredRoleIds { get; set; }
        public ulong resultRoleId { get; set; }
        public bool remain { get; set; }
    }
}