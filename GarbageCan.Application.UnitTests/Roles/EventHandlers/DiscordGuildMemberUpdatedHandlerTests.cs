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
    public class DiscordGuildMemberUpdatedHandlerTests
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

            var logger = _appFixture.GetLogger<DiscordGuildMemberAddedWatchListHandler>();
            logger.ReceivedWithAnyArgs(1).Log(default, default);
        }
    }
}