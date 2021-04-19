using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GarbageCan.Application.UnitTests.Shared.Logging
{
    public static class SubstituteLoggingInjectionExtensions
    {
        public static IServiceCollection AddSubstitutedLogging(this IServiceCollection services)
        {
            var mockedLoggerFactory = new SubstituteLoggerFactory();
            services.AddLogging(builder =>
            {
                builder.AddProvider(mockedLoggerFactory);
            });
            services.AddSingleton(mockedLoggerFactory);
            return services;
        }
    }
}