using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Application.XP.Commands.AddXpToUser;
using GarbageCan.Domain.Entities.XP;
using GarbageCan.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.XP.Commands
{
    public class AddXpToUserCommandTests
    {
        private ApplicationFixture _appFixture;
        private IApplicationDbContext _dbContext;
        private IXpCalculatorService _calculator;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _calculator = Substitute.For<IXpCalculatorService>();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _appFixture.OnConfigureServices += (_, services) =>
            {
                var service = services
                    .First(descriptor => descriptor.ServiceType == typeof(IXpCalculatorService));
                services.Remove(service);
                services.AddSingleton(_calculator);
                services.AddSingleton(_dbContext);
            };
        }

        [Test]
        public async Task ShouldAddXp_WhenUserExists()
        {
            ulong guildId = 6;
            ulong userId = 90;
            var message = "TEST";
            var user = new User
            {
                GuildId = guildId,
                UserId = userId,
                Lvl = 0,
                XP = 0
            };

            _dbContext.ConfigureMockDbSet(x => x.XPUsers, user);
            _calculator.XpEarned(message, guildId).Returns(20.0);

            var command = new AddXpToUserCommand
            {
                GuildId = guildId,
                UserId = userId,
                Message = message
            };

            await _appFixture.SendAsync(command);

            await _dbContext.Received(1).SaveChangesAsync(default);

            _dbContext.XPUsers.First().XP.Should().Be(20);
            await _calculator.Received(1).XpEarned(message, guildId);
        }

        [Test]
        public async Task ShouldCreateNewUser_WhenUserDoesNotExist()
        {
            ulong guildId = 6;
            ulong userId = 90;
            var message = "TEST";

            _calculator.XpEarned(message, guildId).Returns(0);

            User addedUser = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.XPUsers);
            mockDbSet.When(x => x.Add(Arg.Any<User>())).Do(x => addedUser = x.Arg<User>());

            var command = new AddXpToUserCommand
            {
                GuildId = guildId,
                UserId = userId,
                Message = message
            };

            await _appFixture.SendAsync(command);

            _dbContext.XPUsers.Received(1).Add(Arg.Any<User>());
            await _dbContext.Received(1).SaveChangesAsync(default);

            addedUser.Should().NotBeNull();
            addedUser.GuildId.Should().Be(6);
            addedUser.UserId.Should().Be(90);
            addedUser.Lvl.Should().Be(0);
            addedUser.XP.Should().Be(0);
        }

        [Test]
        public async Task ShouldPublishXpAddedEvent_WhenUserExists()
        {
            ulong guildId = 6;
            ulong userId = 90;
            var message = "TEST";
            var user = new User
            {
                GuildId = guildId,
                UserId = userId,
                Lvl = 0,
                XP = 0
            };

            _dbContext.ConfigureMockDbSet(x => x.XPUsers, user);
            _calculator.XpEarned(message, guildId).Returns(20.0);

            var command = new AddXpToUserCommand
            {
                GuildId = guildId,
                UserId = userId,
                Message = message
            };

            await _appFixture.SendAsync(command);

            await _appFixture.Provider.GetRequiredService<IDomainEventService>().Received(1)
                .Publish(Arg.Is<XpAddedToUserEvent>(e =>
                    e.MessageDetails.GuildId == guildId && e.MessageDetails.UserId == userId && e.OldXp == 0 && e.NewXp == 20));
        }

        [Test]
        public async Task ShouldPublishXpAddedEvent_WhenUserDoesNotExist()
        {
            ulong guildId = 6;
            ulong userId = 90;
            var message = "TEST";

            _calculator.XpEarned(message, guildId).Returns(0);

            User addedUser = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.XPUsers);
            mockDbSet.When(x => x.Add(Arg.Any<User>())).Do(x => addedUser = x.Arg<User>());

            var command = new AddXpToUserCommand
            {
                GuildId = guildId,
                UserId = userId,
                Message = message
            };

            await _appFixture.SendAsync(command);

            await _appFixture.Provider.GetRequiredService<IDomainEventService>().Received(1)
                .Publish(Arg.Is<XpAddedToUserEvent>(e =>
                    e.MessageDetails.GuildId == guildId && e.MessageDetails.UserId == userId && e.OldXp == 0 && e.NewXp == 0));
        }
    }
}
