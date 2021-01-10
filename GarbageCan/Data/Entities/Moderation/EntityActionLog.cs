using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GarbageCan.Moderation;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Data.Entities.Moderation
{
    [Table("moderation_warnings")]
    public class EntityActionLog
    {
        [Key] public int id { get; set; }
        public ulong uId { get; set; }
        public ulong mId { get; set; }
        [Column(TypeName = "datetime")] public DateTime issuedDate { get; set; }
        public PunishmentLevel punishmentLevel { get; set; }
        public string comments { get; set; }
    }
}