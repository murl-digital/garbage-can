using GarbageCan.Domain.Common;
using System.Threading.Tasks;

namespace GarbageCan.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}