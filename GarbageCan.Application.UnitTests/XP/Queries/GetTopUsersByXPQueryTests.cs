using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Application.XP.Queries.GetTopUsersByXP;
using GarbageCan.Domain.Entities.XP;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCan.Application.UnitTests.XP.Queries
{
    public class GetTopUsersByXPQueryTests
    {
        private ApplicationFixture _fixture;
        private DbContextFixture _dbContext;
        private Mock<IDiscordGuild> _mock;

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<IDiscordGuild>();

            _fixture = new ApplicationFixture();
            _dbContext = new DbContextFixture();
            _fixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_mock.Object);
                services.AddSingleton(_dbContext.MockContext.Object);
            };
        }

        [Test]
        public async Task ShouldHaveDisplayNameFromGuildCall_WhenGuildIncludesUser()
        {
            ulong currentUserId = 90;
            var displayName = "TEST";
            var currentUser = new EntityUser { Id = currentUserId, Lvl = 100, XP = 20 };
            SetupDisplayNameReturn(displayName, currentUserId);
            _dbContext.XPUsers.Add(currentUser);

            var command = new GetTopUsersByXPQuery
            {
                CurrentUserId = currentUserId,
                Count = 10
            };

            var result = await _fixture.SendAsync(command);

            var first = result.TopTenUsers.First();
            first.User.Should().BeEquivalentTo(currentUser);
            first.DisplayName.Should().Be(displayName);
            _mock.Verify(x => x.GetMemberDisplayNameAsync(currentUserId), Times.Once);
        }

        [Test]
        public async Task ShouldReturnSingleUser_WhenJustOneUserInDbSet()
        {
            ulong currentUserId = 90;
            var displayName = "TEST";
            var currentUser = new EntityUser { Id = currentUserId, Lvl = 100, XP = 20 };
            SetupDisplayNameReturn(displayName, currentUserId);
            _dbContext.XPUsers.Add(currentUser);

            var command = new GetTopUsersByXPQuery
            {
                CurrentUserId = currentUserId,
                Count = 10
            };

            var result = await _fixture.SendAsync(command);

            result.TopTenUsers.Should().NotBeNullOrEmpty();
            result.TopTenUsers.Count.Should().Be(1);
            result.TopTenUsers.First().User.Should().BeEquivalentTo(currentUser);
        }

        [Test]
        public async Task ShouldReturnContextUser_WhenUserIsInDbSet()
        {
            ulong currentUserId = 90;
            var displayName = "TEST";
            var currentUser = new EntityUser { Id = currentUserId, Lvl = 100, XP = 20 };
            SetupDisplayNameReturn(displayName, currentUserId);
            _dbContext.XPUsers.Add(currentUser);

            var command = new GetTopUsersByXPQuery
            {
                CurrentUserId = currentUserId,
                Count = 10
            };

            var result = await _fixture.SendAsync(command);

            result.ContextUser.Should().NotBeNull();
            result.TopTenUsers.Count.Should().Be(1);
            result.ContextUser.User.Should().BeEquivalentTo(currentUser);
        }

        private void SetupDisplayNameReturn(string displayName, ulong userId)
        {
            _mock.Setup(x => x.GetMemberDisplayNameAsync(userId)).ReturnsAsync(displayName);
        }
    }
}