using GarbageCan.Application.UnitTests.Shared.Exceptions;
using GarbageCan.Application.UnitTests.Shared.Logging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Common;
using GarbageCan.Domain.Events;

namespace GarbageCan.Application.UnitTests.Shared
{
    public class MediatorFixture
    {
        private ServiceProvider _provider;
        public EventHandler<IServiceCollection> OnConfigureServices { get; set; }

        public ServiceProvider Provider
        {
            get
            {
                if (_provider != null)
                {
                    return _provider;
                }

                Services.AddSubstitutedLogging();
                OnConfigureServices?.Invoke(this, Services);
                Provider = Services.BuildServiceProvider();

                return _provider;
            }
            private set => _provider = value;
        }

        protected ServiceCollection Services { get; } = new ServiceCollection();

        public SubstituteLogger GetLogger<T>()
        {
            var mockedLoggerFactory = Provider.GetService<SubstituteLoggerFactory>();
            var mockLogger = mockedLoggerFactory.GetLogger<T>();
            return mockLogger;
        }

        public async Task<T> SendAsync<T>(IRequest<T> request)
        {
            var mediator = Provider.GetService<IMediator>();

            if (mediator == null)
            {
                throw new MediatorFixtureConfigurationException();
            }

            return await mediator.Send(request);
        }

        public async Task Publish(INotification notification)
        {
            var mediator = Provider.GetService<IMediator>();

            if (mediator == null)
            {
                throw new MediatorFixtureConfigurationException();
            }

            await mediator.Publish(notification);
        }

        public async Task Publish<T>(T notification) where T : DomainEvent
        {
            await Publish(new DomainEventNotification<T>(notification));
        }
    }
}