using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Application.UnitTests.Shared.Logging;
using GarbageCan.Domain.Entities;
using GarbageCan.Domain.Entities.Roles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using GarbageCan.Application.Roles.Commands.AssignRole;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class AssignRoleTests
    {
        private DbContextFixture _dbContext;
        private ApplicationFixture _fixture;
        private Mock<IDiscordGuildRoleService> _mock;

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<IDiscordGuildRoleService>();

            _dbContext = new DbContextFixture();
            _fixture = new ApplicationFixture();

            _fixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_mock.Object);
                services.AddSingleton(_dbContext.MockContext.Object);
            };
        }

        [Test]
        public async Task ShouldAssignReactionRole_WhenAnExceptionIsThrownForSomeRoles()
        {
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            var badRoleId = 500;
            var goodRoleId = 6944;

            _dbContext.ReactionRoles.Add(CreateReactionRole(channelId, emojiId, messageId, badRoleId));
            _dbContext.ReactionRoles.Add(CreateReactionRole(channelId, emojiId, messageId, goodRoleId));

            var command = CreateCommand(channelId, messageId, emojiId, "TEST");

            _mock.Setup(x => x.GrantRoleAsync(command.GuildId, (ulong)badRoleId, command.UserId, "reaction role")).Throws<Exception>();

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            _mock.Verify(x => x.GrantRoleAsync(command.GuildId, (ulong)goodRoleId, command.UserId, "reaction role"), Times.Once);

            GetMockedLogger().VerifyLogging(LogLevel.Error, "Couldn't assign reaction role", Times.Once);
        }

        [Test]
        public async Task ShouldAssignReactionRole_WhenMultipleRolesMatch()
        {
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            _dbContext.ReactionRoles.Add(CreateReactionRole(channelId, emojiId, messageId, 500));
            _dbContext.ReactionRoles.Add(CreateReactionRole(channelId, emojiId, messageId, 6944));

            var command = CreateCommand(channelId, messageId, emojiId, "TEST");

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            _mock.Verify(x => x.GrantRoleAsync(command.GuildId, 500, command.UserId, "reaction role"), Times.Once);
            _mock.Verify(x => x.GrantRoleAsync(command.GuildId, 6944, command.UserId, "reaction role"), Times.Once);

            GetMockedLogger().VerifyLogging(LogLevel.Error, Times.Never);
        }

        [Theory]
        [TestCase(15, 394, 29, "TEST")]
        [TestCase(15, 394, 0, "29")]
        public async Task ShouldAssignReactionRole_WhenReactionRoleMatchesParameters(int commandChannelId, int commandMessageId, int commandEmojiId, string commandEmojiName)
        {
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            var reactionRole = CreateReactionRole(channelId, emojiId, messageId);

            _dbContext.ReactionRoles.Add(reactionRole);

            var command = CreateCommand(commandChannelId, commandMessageId, commandEmojiId, commandEmojiName);

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            _mock.Verify(x => x.GrantRoleAsync(command.GuildId, reactionRole.roleId, command.UserId, "reaction role"), Times.Once);

            GetMockedLogger().VerifyLogging(LogLevel.Error, Times.Never);
        }

        [Theory]
        [TestCase(50505, 394, 29, "TEST")]
        [TestCase(15, 50505, 29, "TEST")]
        [TestCase(15, 394, 50505, "TEST")]
        [TestCase(15, 394, 0, "9040")]
        public async Task ShouldNotAssignReactionRole_WhenReactionRoleDoesNotMatchesParameters(int commandChannelId, int commandMessageId, int commandEmojiId, string commandEmojiName)
        {
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            var reactionRole = CreateReactionRole(channelId, emojiId, messageId);

            _dbContext.ReactionRoles.Add(reactionRole);

            var command = CreateCommand(commandChannelId, commandMessageId, commandEmojiId, commandEmojiName);

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            _mock.Verify(x => x.GrantRoleAsync(It.IsAny<ulong>(), It.IsAny<ulong>(), It.IsAny<ulong>(), It.IsAny<string>()), Times.Never);

            GetMockedLogger().VerifyLogging(LogLevel.Error, Times.Never);
        }

        private static AssignRoleCommand CreateCommand(int commandChannelId, int commandMessageId, int commandEmojiId, string commandEmojiName)
        {
            return new AssignRoleCommand
            {
                ChannelId = (ulong)commandChannelId,
                MessageId = (ulong)commandMessageId,
                Emoji = new Emoji
                {
                    Id = (ulong)commandEmojiId,
                    Name = commandEmojiName
                },
                GuildId = 45,
                UserId = 4811,
            };
        }

        private static EntityReactionRole CreateReactionRole(int channelId, int emojiId, int messageId, int roleId = 102)
        {
            return new EntityReactionRole
            {
                id = 2,
                roleId = (ulong)roleId,
                channelId = (ulong)channelId,
                emoteId = emojiId.ToString(),
                messageId = (ulong)messageId,
            };
        }

        private MockedILogger<AssignRoleCommandHandler> GetMockedLogger()
        {
            return _fixture.Provider.GetMockedLogger<AssignRoleCommandHandler>();
        }
    }
}