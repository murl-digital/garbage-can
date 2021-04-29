using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Moderation.Commands.Ban;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Moderation;
using GarbageCan.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace GarbageCan.Application.UnitTests.Moderation.Commands
{
    public class BanCommandTests
    {
        private DbSet<ActionLog> _actionLogMock;
        private ApplicationFixture _appFixture;
        private IDateTime _dateTime;
        private IApplicationDbContext _dbContext;
        private IDiscordModerationService _moderationService;
        private IDiscordResponseService _responseService;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _moderationService = Substitute.For<IDiscordModerationService>();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _responseService = Substitute.For<IDiscordResponseService>();
            _dateTime = Substitute.For<IDateTime>();
            _actionLogMock = _dbContext.ConfigureMockDbSet(x => x.ModerationActionLogs);
            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_moderationService);
                services.AddSingleton(_dbContext);
                services.AddSingleton(_responseService);
                services.AddSingleton(_dateTime);
            };
        }

        [Test]
        public async Task ShouldPerformBanningOfMember_WhenValidRequestCalled()
        {
            var command = new BanCommand
            {
                GuildId = 70650,
                UserId = 90,
                MemberId = 14245,
                Reason = "You're Annoying",
                UserDisplayName = "TestUser"
            };

            await _appFixture.SendAsync(command);

            await _moderationService.ReceivedWithAnyArgs(1).BanAsync(default, default);
            await _moderationService.Received(1).BanAsync(command.GuildId, command.UserId, command.Reason);
        }

        [Test]
        public async Task ShouldSaveLogToDb_WhenValidRequestCalled()
        {
            ActionLog addedLog = null;
            _actionLogMock.When(x => x.AddAsync(Arg.Any<ActionLog>())).Do(x => addedLog = x.Arg<ActionLog>());

            var now = DateTime.Now;
            _dateTime.Now.Returns(now);

            var command = new BanCommand
            {
                GuildId = 70650,
                UserId = 90,
                MemberId = 14245,
                Reason = "You're Annoying",
                UserDisplayName = "TestUser"
            };

            await _appFixture.SendAsync(command);

            await _dbContext.ReceivedWithAnyArgs(1).SaveChangesAsync(default);

            await _dbContext.ModerationActionLogs.ReceivedWithAnyArgs(1).AddAsync(default);

            addedLog.Should().NotBeNull();
            addedLog.mId.Should().Be(command.MemberId);
            addedLog.uId.Should().Be(command.UserId);
            addedLog.issuedDate.Should().Be(now.ToUniversalTime());
            addedLog.punishmentLevel.Should().Be(PunishmentLevel.Ban);
            addedLog.comments.Should().Be(command.Reason);
        }

        [Test]
        public async Task ShouldSendResponseMessage_WhenValidRequestCalled()
        {
            var command = new BanCommand
            {
                GuildId = 70650,
                UserId = 90,
                MemberId = 14245,
                Reason = "You're Annoying",
                UserDisplayName = "TestUser"
            };

            await _appFixture.SendAsync(command);

            await _responseService.ReceivedWithAnyArgs(1).RespondAsync(default);
            await _responseService.Received(1).RespondAsync($"{command.UserDisplayName} has been banned", true, false);
        }
    }
}