using System;
using System.Threading.Tasks;
using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.EventHandlers;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Roles;
using GarbageCan.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Roles.EventHandlers
{
    public class DiscordGuildMemberAddedHandlerTests
    {
        private ApplicationFixture _appFixture;
        private IApplicationDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _appFixture.OnConfigureServices += (_, services) => { services.AddSingleton(_dbContext); };
        }

        [Test]
        public async Task ShouldCreateUser_WhenEventIsPublished()
        {
            ulong userId = 90;

            WatchedUser savedUser = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.JoinWatchlist);
            mockDbSet.When(x => x.AddAsync(Arg.Any<WatchedUser>())).Do(x => savedUser = x.Arg<WatchedUser>());

            await _appFixture.Publish(new DiscordGuildMemberAdded
            {
                IsBot = false,
                UserId = userId
            });

            await _dbContext.Received(1).SaveChangesAsync(default);

            savedUser.Should().NotBeNull();
            savedUser.UserId.Should().Be(userId);
        }

        [Test]
        public async Task ShouldNotCreateUser_WhenEventIsPublishedAndUserAlreadyExists()
        {
            ulong userId = 90;

            _dbContext.ConfigureMockDbSet(x => x.JoinWatchlist, new WatchedUser
            {
                UserId = userId
            });

            await _appFixture.Publish(new DiscordGuildMemberAdded
            {
                IsBot = false,
                UserId = userId
            });

            await _dbContext.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
        }

        [Test]
        public async Task ShouldLogError_WhenAnExceptionIsThrown()
        {
            ulong userId = 90;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.JoinWatchlist);
            mockDbSet.When(x => x.AddAsync(Arg.Any<WatchedUser>())).Throw(new Exception("Test Exception"));

            await _appFixture.Publish(new DiscordGuildMemberAdded
            {
                IsBot = false,
                UserId = userId
            });

            var logger = _appFixture.GetLogger<DiscordGuildMemberAddedHandler>();
            logger.ReceivedWithAnyArgs(1).Log(default, default);
        }
    }
}
