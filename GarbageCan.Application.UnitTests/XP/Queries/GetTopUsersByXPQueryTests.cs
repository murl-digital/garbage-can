using Bogus;
using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Application.XP.Queries.GetTopUsersByXP;
using GarbageCan.Domain.Entities.XP;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCan.Application.UnitTests.XP.Queries
{
    public class GetTopUsersByXPQueryTests
    {
        private DbContextFixture _dbContext;
        private ApplicationFixture _fixture;
        private Mock<IDiscordGuildService> _mock;

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<IDiscordGuildService>();

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
        public async Task ShouldReturnTopCountUsers_WhenMoreUsersExistThanAreRequestFromTheCount()
        {
            var users = GenerateUsers(34);
            users.ForEach(x => SetupDisplayNameReturn("TEST", x.Id));

            _dbContext.XPUsers.AddRange(users);

            var count = 10;
            var command = new GetTopUsersByXPQuery
            {
                CurrentUserId = users.First().Id,
                Count = count
            };

            var result = await _fixture.SendAsync(command);

            result.TopTenUsers.Should().NotBeNullOrEmpty();
            result.TopTenUsers.Count.Should().Be(count);
            result.TopTenUsers.First().User.XP.Should().Be(users.Max(x => x.XP));
        }

        private static List<EntityUser> GenerateUsers(int count)
        {
            var faker = new Faker<EntityUser>();
            faker
                .RuleFor(x => x.Id, f => (ulong)f.IndexFaker + 1)
                .RuleFor(x => x.Lvl, f => f.Random.Int(0, 100))
                .RuleFor(x => x.XP, f => f.Random.Double(0, 10000));

            return faker.Generate(count).ToList();
        }

        private void SetupDisplayNameReturn(string displayName, ulong userId)
        {
            _mock.Setup(x => x.GetMemberDisplayNameAsync(userId)).ReturnsAsync(displayName);
        }
    }
}