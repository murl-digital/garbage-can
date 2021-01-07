using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Data.Entities.Moderation
{
    [Table("moderation_warnings")]
    public class EntityWarning
    {
        public ulong uId { get; set; }
        [Column(TypeName = "datetime")] public DateTime issuedDate { get; set; }
        public string comments { get; set; }
    }
}