using FluentAssertions;
using GarbageCan.Application.Common.Configuration;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Moderation.Commands.Mute;
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
    public class MuteCommandTests
    {
        private DbSet<ActionLog> _actionLogMock;
        private DbSet<ActiveMute> _activeMuteMock;
        private ApplicationFixture _appFixture;
        private ulong _currentUserId;
        private ICurrentUserService _currentUserService;
        private IDateTime _dateTime;
        private IApplicationDbContext _dbContext;
        private IDiscordDirectMessageService _directMessageService;
        private IDiscordResponseService _responseService;
        private IRoleConfiguration _roleConfiguration;
        private ulong _roleId;
        private IDiscordGuildRoleService _roleService;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _roleService = Substitute.For<IDiscordGuildRoleService>();
            _responseService = Substitute.For<IDiscordResponseService>();
            _dateTime = Substitute.For<IDateTime>();
            _roleConfiguration = Substitute.For<IRoleConfiguration>();
            _directMessageService = Substitute.For<IDiscordDirectMessageService>();

            _dbContext = Substitute.For<IApplicationDbContext>();
            _actionLogMock = _dbContext.ConfigureMockDbSet(x => x.ModerationActionLogs);
            _activeMuteMock = _dbContext.ConfigureMockDbSet(x => x.ModerationActiveMutes);

            _currentUserService = Substitute.For<ICurrentUserService>();
            _currentUserId = 14245;
            _currentUserService.UserId.Returns(_currentUserId.ToString());

            _roleId = 14245;
            _roleConfiguration.MuteRoleId.Returns(_roleId);

            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_roleService);
                services.AddSingleton(_dbContext);
                services.AddSingleton(_responseService);
                services.AddSingleton(_dateTime);
                services.AddSingleton(_currentUserService);
                services.AddSingleton(_roleConfiguration);
                services.AddSingleton(_directMessageService);
            };
        }

        [Test]
        public async Task ShouldGrantMuteRoleForUser_WhenValidRequestCalled()
        {
            var command = GenerateCommand();

            await _appFixture.SendAsync(command);

            await _roleService.ReceivedWithAnyArgs(1).GrantRoleAsync(default, default, default);
            await _roleService.Received(1).GrantRoleAsync(command.GuildId, _roleId, command.UserId, "user muted for " + command.TimeSpan.Humanize());
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
            addedLog.punishmentLevel.Should().Be(PunishmentLevel.Mute);
            addedLog.comments.Should().Be($"Muted for {command.TimeSpan.Humanize()}. Additional comments: {command.Comments}");
        }

        [Test]
        public async Task ShouldSaveActiveMuteToDb_WhenValidRequestCalled()
        {
            ActiveMute addedLog = null;
            _activeMuteMock.When(x => x.AddAsync(Arg.Any<ActiveMute>())).Do(x => addedLog = x.Arg<ActiveMute>());

            var now = DateTime.Now;
            _dateTime.Now.Returns(now);

            var command = GenerateCommand();

            await _appFixture.SendAsync(command);

            await _dbContext.ReceivedWithAnyArgs(1).SaveChangesAsync(default);
            await _dbContext.ModerationActiveMutes.ReceivedWithAnyArgs(1).AddAsync(default);

            addedLog.Should().NotBeNull();
            addedLog.uId.Should().Be(command.UserId);
            addedLog.expirationDate.Should().Be(_dateTime.Now.ToUniversalTime().Add(command.TimeSpan));
        }

        [Test]
        public async Task ShouldSendDirectMessageToUser_WhenValidRequestCalled()
        {
            var command = GenerateCommand();
            var expectedMessage = $"You have been muted for {command.TimeSpan.Humanize()}.\n\nAdditional comments: {command.Comments}";

            await _appFixture.SendAsync(command);

            await _directMessageService.ReceivedWithAnyArgs(1).SendMessageAsync(default, default);
            await _directMessageService.Received(1).SendMessageAsync(command.UserId, expectedMessage);
        }

        [Test]
        public async Task ShouldSendResponseMessage_WhenValidRequestCalled()
        {
            var command = GenerateCommand();

            await _appFixture.SendAsync(command);

            await _responseService.ReceivedWithAnyArgs(1).RespondAsync(default);
            await _responseService.Received(1).RespondAsync($"{command.UserDisplayName} has been muted", true, false);
        }

        private static MuteCommand GenerateCommand()
        {
            return new MuteCommand
            {
                GuildId = 70650,
                UserId = 90,
                Comments = "You're Annoying",
                UserDisplayName = "TestUser",
                TimeSpan = TimeSpan.FromHours(2)
            };
        }
    }
}