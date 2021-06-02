using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Application.XP.Commands.AddXpToUser;
using GarbageCan.Domain.Entities.XP;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using GarbageCan.Application.XP.Commands.CreateUser;
using GarbageCan.Domain.Events;

namespace GarbageCan.Application.UnitTests.XP.Commands
{
    public class CreateXPUserCommandTests
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
        public async Task ShouldCreateUser_WhenUserDoesNotExist()
        {
            ulong userId = 90;
            User savedUser = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.XPUsers);
            mockDbSet.When(x => x.AddAsync(Arg.Any<User>())).Do(x => savedUser = x.Arg<User>());

            var command = new CreateXPUserCommand
            {
                UserId = userId,
                IsBot = false
            };

            await _appFixture.SendAsync(command);

            await _dbContext.Received(1).SaveChangesAsync(default);

            savedUser.Should().NotBeNull();
            savedUser.Id.Should().Be(userId);
            savedUser.Lvl.Should().Be(0);
            savedUser.XP.Should().Be(0);
        }

        [Test]
        public async Task ShouldNotCreateUser_WhenUserExists()
        {
            ulong userId = 90;
            _dbContext.ConfigureMockDbSet(x => x.XPUsers, new User
            {
                Id = userId
            });

            var command = new CreateXPUserCommand
            {
                UserId = userId,
                IsBot = false
            };

            await _appFixture.SendAsync(command);

            await _dbContext.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
        }

        [Test]
        public async Task ShouldNotCreateUser_WhenUserIsABot()
        {
            ulong userId = 90;
            _dbContext.ConfigureMockDbSet(x => x.XPUsers);

            var command = new CreateXPUserCommand
            {
                UserId = userId,
                IsBot = true
            };

            await _appFixture.SendAsync(command);

            await _dbContext.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
        }
    }
}