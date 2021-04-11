using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GarbageCan.Application.UnitTests.Shared.Logging
{
    public static class MockedLoggingInjectionExtensions
    {
        public static IServiceCollection AddMockedLogging(this IServiceCollection services)
        {
            services.AddSingleton<MockedLoggerFactory>();
            services.AddTransient(typeof(ILogger<>), typeof(MockedPassThroughLogger<>));
            return services;
        }

        public static MockedILogger<T> GetMockedLogger<T>(this IServiceProvider provider)
        {
            return provider.GetRequiredService<MockedLoggerFactory>().GetLogger<T>();
        }

        public static IEnumerable<IMockedLogger> GetAllMockedLoggers(this IServiceProvider provider)
        {
            return provider.GetService<MockedLoggerFactory>().GetMockedLoggers();
        }
    }
}