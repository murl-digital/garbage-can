using GarbageCan.Domain.Enums;
using System;

namespace GarbageCan.Domain.Entities.Moderation
{
    public class ActionLog
    {
        public int id { get; set; }
        public ulong uId { get; init; }
        public ulong mId { get; init; }
        public DateTime issuedDate { get; init; }
        public PunishmentLevel punishmentLevel { get; init; }
        public string comments { get; init; }
    }
}