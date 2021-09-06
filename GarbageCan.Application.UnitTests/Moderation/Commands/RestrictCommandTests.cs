using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Moderation.Commands.Restrict;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Moderation;
using GarbageCan.Domain.Enums;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace GarbageCan.Application.UnitTests.Moderation.Commands
{
    public class RestrictCommandTests
    {
        private DbSet<ActionLog> _actionLogMock;
        private DbSet<ActiveChannelRestrict> _activeChannelRestricts;
        private ApplicationFixture _appFixture;
        private ulong _currentUserId;
        private ICurrentUserService _currentUserService;
        private IDateTime _dateTime;
        private IApplicationDbContext _dbContext;
        private IDiscordDirectMessageService _directMessageService;
        private IDiscordModerationService _moderationService;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _dateTime = Substitute.For<IDateTime>();
            _moderationService = Substitute.For<IDiscordModerationService>();
            _directMessageService = Substitute.For<IDiscordDirectMessageService>();

            _dbContext = Substitute.For<IApplicationDbContext>();
            _actionLogMock = _dbContext.ConfigureMockDbSet(x => x.ModerationActionLogs);
            _activeChannelRestricts = _dbContext.ConfigureMockDbSet(x => x.ModerationActiveChannelRestricts);

            _currentUserService = Substitute.For<ICurrentUserService>();
            _currentUserId = 14245;
            _currentUserService.UserId.Returns(_currentUserId.ToString());

            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_dbContext);
                services.AddSingleton(_dateTime);
                services.AddSingleton(_currentUserService);
                services.AddSingleton(_moderationService);
                services.AddSingleton(_directMessageService);
            };
        }

        [Test]
        public async Task ShouldRestrictChannelAccess_WhenValidRequestCalled()
        {
            var command = GenerateCommand();

            await _appFixture.SendAsync(command);

            await _moderationService.ReceivedWithAnyArgs(1).RestrictChannelAccess(default, default, default);
            await _moderationService.Received(1).RestrictChannelAccess(command.GuildId, command.UserId, command.ChannelId);
        }

        [Test]
        public async Task ShouldSaveActionLogToDb_WhenValidRequestCalled()
        {
            ActionLog addedLog = null;
            _actionLogMock.When(x => x.AddAsync(Arg.Any<ActionLog>())).Do(x => addedLog = x.Arg<ActionLog>());

            var now = DateTime.Now;
            _dateTime.Now.Returns(now);

            var command = GenerateCommand();

            await _appFixture.SendAsync(command);

            await _dbContext.ReceivedWithAnyArgs(1).SaveChangesAsync(default);

            await _dbContext.ModerationActionLogs.ReceivedWithAnyArgs(1).AddAsync(default);

            addedLog.Should().NotBeNull();
            addedLog.mId.Should().Be(_currentUserId);
            addedLog.uId.Should().Be(command.UserId);
            addedLog.issuedDate.Should().Be(now.ToUniversalTime());
            addedLog.punishmentLevel.Should().Be(PunishmentLevel.ChannelRestrict);
            addedLog.comments.Should().Be($"Restricted access to {command.ChannelName} for {command.TimeSpan.Humanize()}. Additional comments: {command.Comments}");
        }

        [Test]
        public async Task ShouldSaveActiveChannelRestrictToDb_WhenValidRequestCalled()
        {
            ActiveChannelRestrict addedLog = null;
            _activeChannelRestricts.When(x => x.AddAsync(Arg.Any<ActiveChannelRestrict>())).Do(x => addedLog = x.Arg<ActiveChannelRestrict>());

            var now = DateTime.Now;
            _dateTime.Now.Returns(now);

            var command = GenerateCommand();

            await _appFixture.SendAsync(command);

            await _dbContext.ReceivedWithAnyArgs(1).SaveChangesAsync(default);
            await _dbContext.ModerationActiveChannelRestricts.ReceivedWithAnyArgs(1).AddAsync(default);

            addedLog.Should().NotBeNull();
            addedLog.uId.Should().Be(command.UserId);
            addedLog.channelId.Should().Be(command.ChannelId);
            addedLog.expirationDate.Should().Be(_dateTime.Now.ToUniversalTime().Add(command.TimeSpan));
        }

        [Test]
        public async Task ShouldSendDirectMessageToUser_WhenValidRequestCalled()
        {
            var command = GenerateCommand();
            var expectedMessage = $"Your access to the {command.ChannelName} channel has been restricted for {command.TimeSpan.Humanize()}.\n\nAdditional comments: {command.Comments}";

            await _appFixture.SendAsync(command);

            await _directMessageService.ReceivedWithAnyArgs(1).SendMessageAsync(default, default);
            await _directMessageService.Received(1).SendMessageAsync(command.UserId, expectedMessage);
        }

        private static RestrictChannelCommand GenerateCommand()
        {
            return new RestrictChannelCommand
            {
                GuildId = 70650,
                UserId = 90,
                ChannelId = 4590,
                ChannelName = "Channel1",
                Comments = "You're Annoying",
                TimeSpan = TimeSpan.FromHours(2)
            };
        }
    }
}
