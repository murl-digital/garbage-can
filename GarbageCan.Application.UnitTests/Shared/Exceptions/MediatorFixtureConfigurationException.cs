using System;

namespace GarbageCan.Application.UnitTests.Shared.Exceptions
{
    public class MediatorFixtureConfigurationException : Exception
    {
        public MediatorFixtureConfigurationException() : base("MediatR was not properly initialized in the service collection")
        {
        }
    }
}