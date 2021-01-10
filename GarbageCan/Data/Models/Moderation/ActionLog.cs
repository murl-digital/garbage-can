using System;
using GarbageCan.Moderation;

namespace GarbageCan.Data.Models.Moderation
{
    public class ActionLog
    {
        public int id { get; set; }
        public ulong uId { get; set; }
        public ulong mId { get; set; }
        public DateTime issuedDate { get; set; }
        public PunishmentLevel punishmentLevel { get; set; }
        public string comments { get; set; }
    }
}