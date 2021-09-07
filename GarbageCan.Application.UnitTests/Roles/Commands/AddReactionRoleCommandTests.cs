using System.Threading.Tasks;
using FluentAssertions;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.ReactionRoles.Commands.AddReactionRole;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities;
using GarbageCan.Domain.Entities.Roles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class AddReactionRoleCommandTests
    {
        private ApplicationFixture _appFixture;
        private IApplicationDbContext _dbContext;
        private IDiscordMessageService _messageService;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _messageService = Substitute.For<IDiscordMessageService>();
            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_dbContext);
                services.AddSingleton(_messageService);
            };
        }

        [Test]
        public async Task ShouldAddReactionRole_WhenNoReactionRolesExist()
        {
            ulong roleId = 5;
            ulong guildId = 69;
            ulong channelId = 215u;
            ulong messageId = 215u;
            var emoji = new Emoji
            {
                Id = 4126u,
                Name = "TEST"
            };

            ReactionRole addedRole = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.ReactionRoles);
            mockDbSet.When(x => x.AddAsync(Arg.Any<ReactionRole>())).Do(x => addedRole = x.Arg<ReactionRole>());

            await _appFixture.SendAsync(new AddReactionRoleCommand
            {
                RoleId = roleId,
                GuildId = guildId,
                ChannelId = channelId,
                MessageId = messageId,
                Emoji = emoji
            });

            await _dbContext.Received(1).SaveChangesAsync(default);
            await _dbContext.ReactionRoles.Received(1).AddAsync(Arg.Any<ReactionRole>());

            addedRole.Should().NotBeNull();
            addedRole.RoleId.Should().Be(roleId);
            addedRole.GuildId.Should().Be(guildId);
            addedRole.ChannelId.Should().Be(channelId);
            addedRole.MessageId.Should().Be(messageId);
        }

        [Test]
        public async Task ShouldCreateReactionRole_WhenAddedSuccessfully()
        {
            _dbContext.ConfigureMockDbSet(x => x.ReactionRoles);
            ulong channelId = 215u;
            ulong messageId = 215u;
            ulong guildId = 90;
            var emoji = new Emoji
            {
                Id = 4126u,
                Name = "TEST"
            };

            await _appFixture.SendAsync(new AddReactionRoleCommand
            {
                GuildId = guildId,
                RoleId = 5,
                ChannelId = channelId,
                MessageId = messageId,
                Emoji = emoji
            });

            await _messageService.Received(1).CreateReactionAsync(guildId, channelId, messageId, emoji);
            await _messageService.Received(1).CreateReactionAsync(Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<ulong>(), Arg.Any<Emoji>());
        }

        [Theory]
        [TestCase(0u, "TEST", "TEST")]
        [TestCase(50u, "TEST", "50")]
        public async Task ShouldProperlySetEmojiValue_WhenNoReactionRolesIsAdded(ulong id, string name, string expected)
        {
            var emoji = new Emoji
            {
                Id = id,
                Name = name
            };

            ReactionRole addedRole = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.ReactionRoles);
            mockDbSet.When(x => x.AddAsync(Arg.Any<ReactionRole>())).Do(x => addedRole = x.Arg<ReactionRole>());

            await _appFixture.SendAsync(new AddReactionRoleCommand
            {
                RoleId = 5,
                ChannelId = 4504,
                MessageId = 5404,
                Emoji = emoji
            });

            addedRole.Should().NotBeNull();
            addedRole.EmoteId.Should().Be(expected);
        }

        [Test]
        public void ShouldThrowValidationException_WhenEmojiIsNull()
        {
            var command = new AddReactionRoleCommand
            {
                RoleId = 50,
                ChannelId = 4561,
                MessageId = 22040,
                Emoji = null
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().ThrowAsync<ValidationException>();
        }

        [Theory]
        [TestCase(0u, 0u, 0u)]
        [TestCase(50u, 0u, 0u)]
        [TestCase(0u, 80u, 0u)]
        [TestCase(0u, 0u, 70u)]
        [TestCase(0u, 30u, 60u)]
        [TestCase(90u, 0u, 60u)]
        [TestCase(90u, 30u, 0u)]
        public void ShouldThrowValidationException_WhenIdIsInvalid(ulong roleId, ulong channelId, ulong messageId)
        {
            var command = new AddReactionRoleCommand
            {
                RoleId = roleId,
                ChannelId = channelId,
                MessageId = messageId,
                Emoji = new Emoji
                {
                    Id = 4126u,
                    Name = "TEST"
                }
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().ThrowAsync<ValidationException>();
        }
    }
}
