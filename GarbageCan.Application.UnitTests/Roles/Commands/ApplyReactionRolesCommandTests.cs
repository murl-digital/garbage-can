using System;
using System.Threading.Tasks;
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

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class ApplyReactionRolesCommandTests
    {
        private IApplicationDbContext _dbContext;
        private ApplicationFixture _fixture;
        private IDiscordGuildRoleService _roleService;
        private SubstituteLogger logger => _fixture.GetLogger<ApplyReactionRolesCommandHandler>();

        [SetUp]
        public void Setup()
        {
            _roleService = Substitute.For<IDiscordGuildRoleService>();
            _dbContext = Substitute.For<IApplicationDbContext>();

            _fixture = new ApplicationFixture();

            _fixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_roleService);
                services.AddSingleton(_dbContext);
            };
        }

        [Test]
        public async Task ShouldAssignReactionRole_WhenAnExceptionIsThrownForSomeRoles()
        {
            var guildId = 16;
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            var badRoleId = 500;
            var goodRoleId = 6944;

            _dbContext.ConfigureMockDbSet(x => x.ReactionRoles,
                new[]
                {
                    CreateReactionRole(guildId, channelId, emojiId, messageId, badRoleId),
                    CreateReactionRole(guildId, channelId, emojiId, messageId, goodRoleId)
                });

            var command = CreateCommand(guildId, channelId, messageId, emojiId, "TEST");

            _roleService.GrantRoleAsync(command.GuildId, (ulong) badRoleId, command.UserId, "reaction role")
                .Throws<Exception>();

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            await _roleService.Received(1)
                .GrantRoleAsync(command.GuildId, (ulong) goodRoleId, command.UserId, "reaction role");

            logger.Received().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());

            await _roleService.DidNotReceive()
                .RevokeRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());
        }

        [Test]
        public async Task ShouldAssignReactionRole_WhenMultipleRolesMatch()
        {
            var guildId = 16;
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            _dbContext.ConfigureMockDbSet(x => x.ReactionRoles,
                new[]
                {
                    CreateReactionRole(guildId, channelId, emojiId, messageId, 500),
                    CreateReactionRole(guildId, channelId, emojiId, messageId, 6944)
                });

            var command = CreateCommand(guildId, channelId, messageId, emojiId, "TEST");

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            await _roleService.Received(1).GrantRoleAsync(command.GuildId, 500, command.UserId, "reaction role");
            await _roleService.Received(1).GrantRoleAsync(command.GuildId, 6944, command.UserId, "reaction role");

            logger.DidNotReceive().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
            await _roleService.DidNotReceive()
                .RevokeRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());
        }

        [Theory]
        [TestCase(15, 394, 29, "TEST")]
        [TestCase(15, 394, 0, "29")]
        public async Task ShouldAssignReactionRole_WhenReactionRoleMatchesParameters(int commandChannelId,
            int commandMessageId, int commandEmojiId, string commandEmojiName)
        {
            var guildId = 16;
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            var reactionRole = CreateReactionRole(guildId, channelId, emojiId, messageId);

            _dbContext.ConfigureMockDbSet(x => x.ReactionRoles, reactionRole);

            var command = CreateCommand(guildId, commandChannelId, commandMessageId, commandEmojiId, commandEmojiName);

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            await _roleService.Received(1)
                .GrantRoleAsync(command.GuildId, reactionRole.RoleId, command.UserId, "reaction role");
            await _roleService.DidNotReceive()
                .RevokeRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());

            logger.DidNotReceive().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
        }

        [Theory]
        [TestCase(50505, 394, 29, "TEST")]
        [TestCase(15, 50505, 29, "TEST")]
        [TestCase(15, 394, 50505, "TEST")]
        [TestCase(15, 394, 0, "9040")]
        public async Task ShouldNotAssignReactionRole_WhenReactionRoleDoesNotMatchesParameters(int commandChannelId,
            int commandMessageId, int commandEmojiId, string commandEmojiName)
        {
            var guildId = 16;
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            var reactionRole = CreateReactionRole(guildId, channelId, emojiId, messageId);

            _dbContext.ConfigureMockDbSet(x => x.ReactionRoles, reactionRole);

            var command = CreateCommand(guildId, commandChannelId, commandMessageId, commandEmojiId, commandEmojiName);

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            await _roleService.DidNotReceive()
                .GrantRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());
            await _roleService.DidNotReceive()
                .RevokeRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());

            logger.DidNotReceive().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
        }

        [Theory]
        [TestCase(50505, 394, 29, "TEST")]
        [TestCase(15, 50505, 29, "TEST")]
        [TestCase(15, 394, 50505, "TEST")]
        [TestCase(15, 394, 0, "9040")]
        public async Task ShouldNotRevokeReactionRole_WhenReactionRoleDoesNotMatchesParameters(int commandChannelId,
            int commandMessageId, int commandEmojiId, string commandEmojiName)
        {
            var guildId = 16;
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            var reactionRole = CreateReactionRole(guildId, channelId, emojiId, messageId);

            _dbContext.ConfigureMockDbSet(x => x.ReactionRoles, reactionRole);

            var command = CreateCommand(guildId, commandChannelId, commandMessageId, commandEmojiId, commandEmojiName,
                false);

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            await _roleService.DidNotReceive()
                .RevokeRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());
            await _roleService.DidNotReceive()
                .GrantRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());

            logger.DidNotReceive().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
        }

        [Test]
        public async Task ShouldRevokeReactionRole_WhenAnExceptionIsThrownForSomeRoles()
        {
            var guildId = 16;
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            var badRoleId = 500;
            var goodRoleId = 6944;

            _dbContext.ConfigureMockDbSet(x => x.ReactionRoles,
                new[]
                {
                    CreateReactionRole(guildId, channelId, emojiId, messageId, badRoleId),
                    CreateReactionRole(guildId, channelId, emojiId, messageId, goodRoleId)
                });

            var command = CreateCommand(guildId, channelId, messageId, emojiId, "TEST", false);

            _roleService.RevokeRoleAsync(command.GuildId, (ulong) badRoleId, command.UserId, "reaction role")
                .Throws<Exception>();

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();

            await _roleService.Received(1)
                .RevokeRoleAsync(command.GuildId, (ulong) goodRoleId, command.UserId, "reaction role");
            await _roleService.DidNotReceive()
                .GrantRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());

            logger.Received(1).Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
        }

        [Test]
        public async Task ShouldRevokeReactionRole_WhenMultipleRolesMatch()
        {
            var guildId = 16;
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            _dbContext.ConfigureMockDbSet(x => x.ReactionRoles,
                new[]
                {
                    CreateReactionRole(guildId, channelId, emojiId, messageId, 500),
                    CreateReactionRole(guildId, channelId, emojiId, messageId, 6944)
                });

            var command = CreateCommand(guildId, channelId, messageId, emojiId, "TEST", false);

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            await _roleService.Received(1).RevokeRoleAsync(command.GuildId, 500, command.UserId, "reaction role");
            await _roleService.Received(1).RevokeRoleAsync(command.GuildId, 6944, command.UserId, "reaction role");

            await _roleService.DidNotReceive()
                .GrantRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());

            logger.DidNotReceive().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
        }

        [Theory]
        [TestCase(15, 394, 29, "TEST")]
        [TestCase(15, 394, 0, "29")]
        public async Task ShouldRevokeReactionRole_WhenReactionRoleMatchesParameters(int commandChannelId,
            int commandMessageId, int commandEmojiId, string commandEmojiName)
        {
            var guildId = 16;
            var channelId = 15;
            var messageId = 394;
            var emojiId = 29;

            var reactionRole = CreateReactionRole(guildId, channelId, emojiId, messageId);

            _dbContext.ConfigureMockDbSet(x => x.ReactionRoles, reactionRole);

            var command = CreateCommand(guildId, commandChannelId, commandMessageId, commandEmojiId, commandEmojiName,
                false);

            var result = await _fixture.SendAsync(command);

            result.Should().BeTrue();
            await _roleService.Received(1)
                .RevokeRoleAsync(command.GuildId, reactionRole.RoleId, command.UserId, "reaction role");
            await _roleService.DidNotReceive()
                .GrantRoleAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<string>());

            logger.DidNotReceive().Log(Arg.Is<LogLevel>(x => x == LogLevel.Error), Arg.Any<string>());
        }

        private static ApplyReactionRolesCommand CreateCommand(int commandGuildId, int commandChannelId,
            int commandMessageId,
            int commandEmojiId,
            string commandEmojiName, bool add = true)
        {
            return new()
            {
                ChannelId = (ulong) commandChannelId,
                MessageId = (ulong) commandMessageId,
                Emoji = new Emoji
                {
                    Id = (ulong) commandEmojiId,
                    Name = commandEmojiName
                },
                GuildId = (ulong) commandGuildId,
                UserId = 4811,
                Add = add
            };
        }

        private static ReactionRole CreateReactionRole(int guildId, int channelId, int emojiId, int messageId,
            int roleId = 102)
        {
            return new()
            {
                Id = 2,
                GuildId = (ulong) guildId,
                RoleId = (ulong) roleId,
                ChannelId = (ulong) channelId,
                EmoteId = emojiId.ToString(),
                MessageId = (ulong) messageId
            };
        }
    }
}
