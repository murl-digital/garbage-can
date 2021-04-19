using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.Commands.AlterRole;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Application.UnitTests.Shared.Logging;
using GarbageCan.Domain.Entities;
using GarbageCan.Domain.Entities.Roles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class AlterRoleTests
    {
        private DbContextFixture _dbContext;
        private ApplicationFixture _fixture;
        private IDiscordGuildRoleService _roleService;
        private SubstituteLogger logger => _fixture.GetLogger<AlterRoleCommandHandler>();

        [SetUp]
        public void Setup()
        {
            _roleService = Substitute.For<IDiscordGuildRoleService>();

            _dbContext = new DbContextFixture();
            _fixture = new ApplicationFixture();

            _fixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_roleService);
                services.AddSingleton(_dbContext.MockContext);
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

            _roleService.GrantRoleAsync(command.GuildId, (ulong)badRoleId, command.UserId, "reaction role").Throws<Exception>();

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            await _roleService.Received(1).GrantRoleAsync(command.GuildId, (ulong)goodRoleId, command.UserId, "reaction role");

            logger.Received().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());

            await _roleService.DidNotReceive().RevokeRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());
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
            await _roleService.Received(1).GrantRoleAsync(command.GuildId, 500, command.UserId, "reaction role");
            await _roleService.Received(1).GrantRoleAsync(command.GuildId, 6944, command.UserId, "reaction role");

            logger.DidNotReceive().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
            await _roleService.DidNotReceive().RevokeRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());
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
            await _roleService.Received(1).GrantRoleAsync(command.GuildId, reactionRole.roleId, command.UserId, "reaction role");
            await _roleService.DidNotReceive().RevokeRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());

            logger.DidNotReceive().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
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
            await _roleService.DidNotReceive().GrantRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());
            await _roleService.DidNotReceive().RevokeRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());

            logger.DidNotReceive().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
        }

        [Theory]
        [TestCase(50505, 394, 29, "TEST")]
        [TestCase(15, 50505, 29, "TEST")]
        [TestCase(15, 394, 50505, "TEST")]
        [TestCase(15, 394, 0, "9040")]
        public async Task ShouldNotRevokeReactionRole_WhenReactionRoleDoesNotMatchesParameters(int commandChannelId, int commandMessageId, int commandEmojiId, string commandEmojiName)
        {
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            var reactionRole = CreateReactionRole(channelId, emojiId, messageId);

            _dbContext.ReactionRoles.Add(reactionRole);

            var command = CreateCommand(commandChannelId, commandMessageId, commandEmojiId, commandEmojiName, false);

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            await _roleService.DidNotReceive().RevokeRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());
            await _roleService.DidNotReceive().GrantRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());

            logger.DidNotReceive().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
        }

        [Test]
        public async Task ShouldRevokeReactionRole_WhenAnExceptionIsThrownForSomeRoles()
        {
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            var badRoleId = 500;
            var goodRoleId = 6944;

            _dbContext.ReactionRoles.Add(CreateReactionRole(channelId, emojiId, messageId, badRoleId));
            _dbContext.ReactionRoles.Add(CreateReactionRole(channelId, emojiId, messageId, goodRoleId));

            var command = CreateCommand(channelId, messageId, emojiId, "TEST", false);

            _roleService.RevokeRoleAsync(command.GuildId, (ulong)badRoleId, command.UserId, "reaction role").Throws<Exception>();

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();

            await _roleService.Received(1).RevokeRoleAsync(command.GuildId, (ulong)goodRoleId, command.UserId, "reaction role");
            await _roleService.DidNotReceive().GrantRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());

            logger.Received(1).Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
        }

        [Test]
        public async Task ShouldRevokeReactionRole_WhenMultipleRolesMatch()
        {
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            _dbContext.ReactionRoles.Add(CreateReactionRole(channelId, emojiId, messageId, 500));
            _dbContext.ReactionRoles.Add(CreateReactionRole(channelId, emojiId, messageId, 6944));

            var command = CreateCommand(channelId, messageId, emojiId, "TEST", false);

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            await _roleService.Received(1).RevokeRoleAsync(command.GuildId, 500, command.UserId, "reaction role");
            await _roleService.Received(1).RevokeRoleAsync(command.GuildId, 6944, command.UserId, "reaction role");

            await _roleService.DidNotReceive().GrantRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());

            logger.DidNotReceive().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
        }

        [Theory]
        [TestCase(15, 394, 29, "TEST")]
        [TestCase(15, 394, 0, "29")]
        public async Task ShouldRevokeReactionRole_WhenReactionRoleMatchesParameters(int commandChannelId, int commandMessageId, int commandEmojiId, string commandEmojiName)
        {
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            var reactionRole = CreateReactionRole(channelId, emojiId, messageId);

            _dbContext.ReactionRoles.Add(reactionRole);

            var command = CreateCommand(commandChannelId, commandMessageId, commandEmojiId, commandEmojiName, false);

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            await _roleService.Received(1).RevokeRoleAsync(command.GuildId, reactionRole.roleId, command.UserId, "reaction role");
            await _roleService.DidNotReceive().GrantRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());

            logger.DidNotReceive().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
        }

        private static AlterRoleCommand CreateCommand(int commandChannelId, int commandMessageId, int commandEmojiId, string commandEmojiName, bool add = true)
        {
            return new AlterRoleCommand
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
                Add = add
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
    }
}