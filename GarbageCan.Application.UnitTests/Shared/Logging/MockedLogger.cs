using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;

namespace GarbageCan.Application.UnitTests.Shared.Logging
{
    public class MockedILogger<T> : Mock<ILogger<T>>, IMockedLogger
    {
    }

    public interface IMockedLogger
    {
    }

    internal class MockedLoggerFactory
    {
        private readonly Dictionary<string, IMockedLogger> _loggers = new Dictionary<string, IMockedLogger>();

        public MockedILogger<T> GetLogger<T>()
        {
            var fullName = typeof(T).FullName ?? throw new TypeAccessException("Couldn't get Generic type name");

            if (!_loggers.ContainsKey(fullName))
            {
                var mockLogger = new MockedILogger<T>();
                _loggers.Add(fullName, mockLogger);
            }

            return _loggers[fullName] as MockedILogger<T>;
        }

        public IEnumerable<IMockedLogger> GetMockedLoggers()
        {
            return _loggers.Values;
        }
    }

    internal class MockedPassThroughLogger<T> : ILogger<T>
    {
        private readonly MockedILogger<T> _mockedILogger;

        public MockedPassThroughLogger(MockedLoggerFactory factory)
        {
            _mockedILogger = factory.GetLogger<T>();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _mockedILogger.Object.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _mockedILogger.Object.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _mockedILogger.Object.BeginScope(state);
        }
    }
}