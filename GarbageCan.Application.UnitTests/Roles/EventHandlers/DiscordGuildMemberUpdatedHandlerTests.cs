using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.JoinRoles.EventHandlers;
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
        private IDiscordGuildRoleService _roleService;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _roleService = Substitute.For<IDiscordGuildRoleService>();
            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_dbContext);
                services.AddSingleton(_roleService);
            };
        }

        [Test]
        public async Task ShouldLogError_WhenAnExceptionIsThrown()
        {
            ulong userId = 90;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.JoinWatchlist, new WatchedUser {UserId = userId});
            mockDbSet.When(x => x.Remove(Arg.Any<WatchedUser>())).Throw(new Exception("Test Exception"));

            await _appFixture.Publish(new DiscordGuildMemberUpdated
            {
                IsBot = false,
                UserId = userId,
                IsPending = false
            });

            var logger = _appFixture.GetLogger<DiscordGuildMemberUpdatedHandler>();
            logger.ReceivedWithAnyArgs(1).Log(default, default);
        }

        [Test]
        public async Task ShouldDoNothing_WhenUserIsNotInWatchList()
        {
            ulong userId = 90;
            ulong otherUserId = 95;
            _dbContext.ConfigureMockDbSet(x => x.JoinWatchlist, new WatchedUser {UserId = otherUserId});

            await _appFixture.Publish(new DiscordGuildMemberUpdated
            {
                IsBot = false,
                UserId = userId
            });

            await _dbContext.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
            await _roleService.DidNotReceiveWithAnyArgs().GrantRoleAsync(default, default, default);
        }

        [Test]
        public async Task ShouldDoNothing_WhenUserIsABot()
        {
            ulong userId = 90;
            ulong otherUserId = 95;
            _dbContext.ConfigureMockDbSet(x => x.JoinWatchlist, new WatchedUser {UserId = otherUserId});

            await _appFixture.Publish(new DiscordGuildMemberUpdated
            {
                IsBot = true,
                UserId = userId
            });

            await _dbContext.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
            await _roleService.DidNotReceiveWithAnyArgs().GrantRoleAsync(default, default, default);
        }


        [Test]
        [TestCase(true)]
        [TestCase(null)]
        public async Task ShouldDoNothing_WhenUserIsPendingOrNull(bool? pending)
        {
            ulong userId = 90;
            var user = new WatchedUser {UserId = userId};
            _dbContext.ConfigureMockDbSet(x => x.JoinWatchlist, user);

            await _appFixture.Publish(new DiscordGuildMemberUpdated
            {
                IsBot = false,
                IsPending = pending,
                UserId = userId
            });

            await _dbContext.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
            await _roleService.DidNotReceiveWithAnyArgs().GrantRoleAsync(default, default, default);
        }

        [Test]
        public async Task ShouldRemoveFromWatchList_WhenUserIsInWatchList()
        {
            ulong userId = 90;
            var user = new WatchedUser {UserId = userId};
            _dbContext.ConfigureMockDbSet(x => x.JoinWatchlist, user);

            await _appFixture.Publish(new DiscordGuildMemberUpdated
            {
                IsBot = false,
                UserId = userId,
                IsPending = false
            });

            _dbContext.JoinWatchlist.Received(1).Remove(user);
            await _dbContext.ReceivedWithAnyArgs(1).SaveChangesAsync(default);
        }

        [Test]
        public async Task ShouldGrantJoinRole_WhenUserIsInWatchList()
        {
            ulong userId = 90;
            ulong guildId = 51;
            var user = new WatchedUser {GuildId = guildId, UserId = userId};
            _dbContext.ConfigureMockDbSet(x => x.JoinWatchlist, user);

            var roles = new List<JoinRole>
                {new() {GuildId = guildId, RoleId = 9}, new() {GuildId = guildId, RoleId = 942}};
            _dbContext.ConfigureMockDbSet(x => x.JoinRoles, roles);

            await _appFixture.Publish(new DiscordGuildMemberUpdated
            {
                IsBot = false,
                UserId = userId,
                GuildId = guildId,
                IsPending = false
            });

            foreach (var joinRole in roles)
                await _roleService.Received(1).GrantRoleAsync(guildId, joinRole.RoleId, userId, "join role");
        }
    }
}
