using FluentAssertions;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Application.XP.Queries.GetTopUsersByXP;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.XP.Queries
{
    public class GetTopUsersByXPQueryValidationTests
    {
        private readonly ApplicationFixture _fixture;

        public GetTopUsersByXPQueryValidationTests()
        {
            _fixture = new ApplicationFixture();
        }

        [Theory]
        [TestCase(0u)]
        public void ShouldThrowValidationException_WhenCurrentUserIdIsInvalid(ulong id)
        {
            var command = new GetTopUsersByXPQuery
            {
                CurrentUserId = id,
                Count = 10
            };

            FluentActions.Invoking(() => _fixture.SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Theory]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void ShouldThrowValidationException_WhenCountIsInvalid(int count)
        {
            var command = new GetTopUsersByXPQuery
            {
                CurrentUserId = 84,
                Count = count
            };

            FluentActions.Invoking(() => _fixture.SendAsync(command)).Should().Throw<ValidationException>();
        }
    }
}