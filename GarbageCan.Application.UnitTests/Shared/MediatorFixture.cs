using GarbageCan.Application.UnitTests.Shared.Exceptions;
using GarbageCan.Application.UnitTests.Shared.Logging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace GarbageCan.Application.UnitTests.Shared
{
    public class MediatorFixture
    {
        public ServiceProvider Provider { get; private set; }

        protected ServiceCollection Services { get; } = new ServiceCollection();

        public EventHandler<IServiceCollection> OnConfigureServices { get; set; }

        public MockedILogger<T> GetMockedLogger<T>()
        {
            return Provider.GetMockedLogger<T>();
        }

        public async Task<T> SendAsync<T>(IRequest<T> request)
        {
            if (Provider == null)
            {
                Services.AddMockedLogging();
                OnConfigureServices?.Invoke(this, Services);
                Provider = Services.BuildServiceProvider();
            }

            var mediator = Provider.GetService<IMediator>();

            if (mediator == null)
            {
                throw new MediatorFixtureConfigurationException();
            }

            return await mediator.Send(request);
        }
    }
}