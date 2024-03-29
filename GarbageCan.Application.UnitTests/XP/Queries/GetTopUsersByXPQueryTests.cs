﻿using Bogus;
using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Application.XP.Queries.GetTopUsersByXP;
using GarbageCan.Domain.Entities.XP;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCan.Application.UnitTests.XP.Queries
{
    public class GetTopUsersByXPQueryTests
    {
        private ApplicationFixture _fixture;
        private IApplicationDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            _dbContext = Substitute.For<IApplicationDbContext>();

            _fixture = new ApplicationFixture();
            _fixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_dbContext);
            };
        }

        [Test]
        public async Task ShouldReturnContextUser_WhenUserIsInDbSet()
        {
            ulong currentUserId = 90;
            var currentUser = new User { UserId = currentUserId, Lvl = 100, XP = 20 };

            _dbContext.ConfigureMockDbSet(x => x.XPUsers, currentUser);

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
            var currentUser = new User { UserId = currentUserId, Lvl = 100, XP = 20 };

            _dbContext.ConfigureMockDbSet(x => x.XPUsers, currentUser);

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

            _dbContext.ConfigureMockDbSet(x => x.XPUsers, users);

            var count = 10;
            var command = new GetTopUsersByXPQuery
            {
                CurrentUserId = users.First().UserId,
                Count = count
            };

            var result = await _fixture.SendAsync(command);

            result.TopTenUsers.Should().NotBeNullOrEmpty();
            result.TopTenUsers.Count.Should().Be(count);
            result.TopTenUsers.First().User.XP.Should().Be(users.Max(x => x.XP));
        }

        private static List<User> GenerateUsers(int count)
        {
            var faker = new Faker<User>();
            faker
                .RuleFor(x => x.UserId, f => (ulong)f.IndexFaker + 1)
                .RuleFor(x => x.Lvl, f => f.Random.Int(0, 100))
                .RuleFor(x => x.XP, f => f.Random.Double(0, 10000));

            return faker.Generate(count).ToList();
        }
    }
}
