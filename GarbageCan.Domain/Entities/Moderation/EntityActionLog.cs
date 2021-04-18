using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GarbageCan.Domain.Enums;

namespace GarbageCan.Domain.Entities.Moderation
{
    public class EntityActionLog
    {
        [Key] public int id { get; set; }
        public ulong uId { get; init; }
        public ulong mId { get; init; }
        [Column(TypeName = "datetime")] public DateTime issuedDate { get; init; }
        public PunishmentLevel punishmentLevel { get; init; }
        public string comments { get; init; }
    }
}