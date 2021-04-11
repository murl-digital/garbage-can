using GarbageCan.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace GarbageCan.Application.UnitTests.Shared
{
    public class ApplicationFixture : MediatorFixture
    {
        public ApplicationFixture()
        {
            OnConfigureServices += (_, services) =>
            {
                services.AddApplication();

                // Mock Infrastructure provided implementations
                services.AddSingleton(Mock.Of<ICurrentUserService>());
                services.AddSingleton(Mock.Of<IDomainEventService>());
            };
        }
    }
}