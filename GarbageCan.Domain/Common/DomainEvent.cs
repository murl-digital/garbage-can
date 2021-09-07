using System;

namespace GarbageCan.Domain.Common
{
    public abstract class DomainEvent
    {
        protected DomainEvent()
        {
            DateOccurred = DateTimeOffset.UtcNow;
            EventId = Guid.NewGuid();
        }

        public DateTimeOffset DateOccurred { get; protected set; }
        public Guid EventId { get; protected set; }
    }
}