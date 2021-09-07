using GarbageCan.Domain.Common;
using GarbageCan.Domain.Enums;

namespace GarbageCan.Domain.Events
{
    public class DiscordConnectionChangeEvent : DomainEvent
    {
        public DiscordConnectionStatus Status { get; set; }
    }
}