using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Fun.Commands.OobifyText;
using GarbageCan.Application.UnitTests.Shared;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;
using NSubstitute;

namespace GarbageCan.Application.UnitTests.Fun.Commands
{
    public class OobifyTextCommandTests
    {
        private ApplicationFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new ApplicationFixture();
        }

        [Theory]
        [TestCase("test", "toobst")]
        public async Task ShouldRespondWithOobifyedMessage_WhenFullWordIsPassed(string text, string expected)
        {
            await PerformOobifyTest(text, expected);
        }

        [Theory]
        [TestCase("test 5 test", "toobst 5 toobst")]
        public async Task ShouldRespondWithOobifyedMessage_WhenMultipleWordsArePassed(string text, string expected)
        {
            await PerformOobifyTest(text, expected);
        }

        [Theory]
        [TestCase("a", "oob")]
        [TestCase("e", "oob")]
        [TestCase("i", "oob")]
        [TestCase("o", "oob")]
        [TestCase("u", "oob")]
        [TestCase("A", "Oob")]
        [TestCase("E", "Oob")]
        [TestCase("I", "Oob")]
        [TestCase("O", "Oob")]
        [TestCase("U", "Oob")]
        public async Task ShouldRespondWithOobifyedMessage_WhenSingleValidLetterIsPassed(string text, string expected)
        {
            await PerformOobifyTest(text, expected);
        }

        private async Task PerformOobifyTest(string text, string expected)
        {
            var result = await _fixture.SendAsync(new OobifyTextCommand { Text = text });

            result.Should().Be(expected);
        }
    }
}
