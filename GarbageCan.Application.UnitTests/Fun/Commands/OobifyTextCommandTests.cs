using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Fun.Commands.OobifyText;
using GarbageCan.Application.UnitTests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace GarbageCan.Application.UnitTests.Fun.Commands
{
    public class OobifyTextCommandTests
    {
        private ApplicationFixture _fixture;
        private Mock<IDiscordResponseService> _mock;

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<IDiscordResponseService>();

            _fixture = new ApplicationFixture();

            _fixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_mock.Object);
            };
        }

        [Theory]
        [TestCase("test", "tooboobbst")]
        public async Task ShouldRespondWithOobifyedMessage_WhenFullWordIsPassed(string text, string expected)
        {
            await PerformOobifyTest(text, expected);
        }

        [Theory]
        [TestCase("test 5 test", "tooboobbst 5 tooboobbst")]
        public async Task ShouldRespondWithOobifyedMessage_WhenMultipleWordsArePassed(string text, string expected)
        {
            await PerformOobifyTest(text, expected);
        }

        [Theory]
        [TestCase("a", "ooboobb")]
        [TestCase("e", "ooboobb")]
        [TestCase("i", "ooboobb")]
        [TestCase("o", "oob")]
        [TestCase("u", "oob")]
        [TestCase("A", "Oobob")]
        [TestCase("E", "Oobob")]
        [TestCase("I", "Oobob")]
        [TestCase("O", "Oob")]
        [TestCase("U", "Oob")]
        public async Task ShouldRespondWithOobifyedMessage_WhenSingleValidLetterIsPassed(string text, string expected)
        {
            await PerformOobifyTest(text, expected);
        }

        private async Task PerformOobifyTest(string text, string expected)
        {
            var result = await _fixture.SendAsync(new OobifyTextCommand { Text = text });

            result.Should().BeTrue();

            _mock.Verify(x => x.RespondAsync(expected, false, false), Times.Once);
            _mock.Verify(x => x.RespondAsync(It.IsNotIn(expected), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        }
    }
}