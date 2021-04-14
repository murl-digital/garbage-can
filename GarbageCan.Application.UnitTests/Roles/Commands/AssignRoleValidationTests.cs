using FluentAssertions;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Reactions.Commands.AssignRole;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class AssignRoleValidationTests
    {
        private readonly ApplicationFixture _fixture;

        public AssignRoleValidationTests()
        {
            _fixture = new ApplicationFixture();
        }

        [Theory]
        public void ShouldThrowValidationException_WhenEmojiIsNull()
        {
            var command = new AssignRoleCommand
            {
                GuildId = 151,
                UserId = 450,
                ChannelId = 4540,
                MessageId = 500,
                Emoji = null,
            };

            FluentActions.Invoking(() => _fixture.SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Theory]
        [TestCase(0u, 41u, 41u, 41u, 41u, "TEST")]
        [TestCase(41u, 0u, 41u, 41u, 41u, "TEST")]
        [TestCase(41u, 41u, 0u, 41u, 41u, "TEST")]
        [TestCase(41u, 41u, 41u, 0u, 41u, "TEST")]
        [TestCase(41u, 41u, 41u, 41u, 0u, null)]
        [TestCase(41u, 41u, 41u, 41u, 0u, "")]
        [TestCase(41u, 41u, 41u, 41u, 0u, "   ")]
        public void ShouldThrowValidationException_WhenHasInvalidProperty(ulong guildId, ulong userId, ulong channelId, ulong messageId, ulong emojiId, string emojiName)
        {
            var command = new AssignRoleCommand
            {
                GuildId = guildId,
                UserId = userId,
                ChannelId = channelId,
                MessageId = messageId,
                Emoji = new Emoji
                {
                    Id = emojiId,
                    Name = emojiName,
                },
            };

            FluentActions.Invoking(() => _fixture.SendAsync(command)).Should().Throw<ValidationException>();
        }
    }
}