using System;
using System.Threading.Tasks;
using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Application.XP.EventHandlers;
using GarbageCan.Domain.Entities.XP;
using GarbageCan.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.XP.EventHandlers
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
        public async Task ShouldSaveUser_WhenEventIsPublished()
        {
            ulong userId = 90;
            User savedUser = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.XPUsers);
            mockDbSet.When(x => x.AddAsync(Arg.Any<User>())).Do(x => savedUser = x.Arg<User>());

            await _appFixture.Publish(new DiscordGuildMemberAdded
            {
                IsBot = false,
                UserId = userId
            });

            await _dbContext.Received(1).SaveChangesAsync(default);

            savedUser.Should().NotBeNull();
            savedUser.UserId.Should().Be(userId);
            savedUser.Lvl.Should().Be(0);
            savedUser.XP.Should().Be(0);
        }

        [Test]
        public async Task ShouldLogError_WhenAnExceptionIsThrown()
        {
            ulong userId = 90;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.XPUsers);
            mockDbSet.When(x => x.AddAsync(Arg.Any<User>())).Throw(new Exception("Test Exception"));

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