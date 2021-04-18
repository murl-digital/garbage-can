using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Domain.Entities.Moderation
{
    public class EntityActiveMute
    {
        [Key] public int id { get; set; }
        public ulong uId { get; init; }
        [Column(TypeName = "datetime")] public DateTime expirationDate { get; init; }
    }
}