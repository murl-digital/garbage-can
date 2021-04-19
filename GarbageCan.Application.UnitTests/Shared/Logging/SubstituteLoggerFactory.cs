using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace GarbageCan.Application.UnitTests.Shared.Logging
{
    internal class SubstituteLoggerFactory : ILoggerProvider
    {
        private readonly Dictionary<string, SubstituteLogger> _loggers = new Dictionary<string, SubstituteLogger>();

        public ILogger CreateLogger(string categoryName)
        {
            if (!_loggers.ContainsKey(categoryName))
            {
                var mockLogger = Substitute.For<SubstituteLogger>();
                _loggers.Add(categoryName, mockLogger);
            }

            return _loggers[categoryName];
        }

        public void Dispose()
        {
        }

        public SubstituteLogger GetLogger<T>()
        {
            var fullName = typeof(T).FullName ?? throw new TypeAccessException("Couldn't get Generic type name");

            if (!_loggers.ContainsKey(fullName))
            {
                throw new ArgumentException("No Logger Exists in dictionary");
            }

            return _loggers[fullName];
        }
    }
}