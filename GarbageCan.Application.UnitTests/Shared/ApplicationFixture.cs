using GarbageCan.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

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
                services.AddSingleton(Substitute.For<ICurrentUserService>());
                services.AddSingleton(Substitute.For<IDomainEventService>());
            };
        }
    }
}