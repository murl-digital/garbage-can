using Bogus;
using FluentAssertions;
using GarbageCan.Application.Common.Configuration;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Moderation.Commands.Mute;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Moderation;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCan.Application.UnitTests.Moderation.Commands
{
    public class UnMuteExpiredMutesCommandTests
    {
        private readonly ulong _roleId = 879400;
        private ApplicationFixture _appFixture;
        private IDateTime _dateTime;
        private IApplicationDbContext _dbContext;
        private IDiscordConfiguration _discordConfiguration;
        private IRoleConfiguration _roleConfiguration;
        private IDiscordGuildRoleService _roleService;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _dateTime = Substitute.For<IDateTime>();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _roleService = Substitute.For<IDiscordGuildRoleService>();
            _discordConfiguration = Substitute.For<IDiscordConfiguration>();
            
            _roleConfiguration = Substitute.For<IRoleConfiguration>();
            _roleConfiguration.MuteRoleId.Returns(_roleId);

            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_dbContext);
                services.AddSingleton(_dateTime);
                services.AddSingleton(_roleService);
                services.AddSingleton(_discordConfiguration);
                services.AddSingleton(_roleConfiguration);
            };
        }

        [Theory]
        [TestCase(1)]
        [TestCase(60)]
        [TestCase(60 * 24)]
        public async Task ShouldNotRevokeAnyRoles_WhenMutesThatAreNotExpiredExist(int secondsInFuture)
        {
            var now = DateTime.Now;
            _dateTime.Now.Returns(now);

            var activeMute = GenerateActiveMute(now.AddSeconds(secondsInFuture));
            _dbContext.ConfigureMockDbSet(x => x.ModerationActiveMutes, activeMute);

            await _appFixture.SendAsync(new UnMuteExpiredMutesCommand());

            await _roleService.DidNotReceiveWithAnyArgs().RevokeRoleAsync(default, default, default);
            await _dbContext.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
        }

        [Test]
        public async Task ShouldNotRevokeAnyRoles_WhenNoMutesExist()
        {
            _dbContext.ConfigureMockDbSet(x => x.ModerationActiveMutes);

            await _appFixture.SendAsync(new UnMuteExpiredMutesCommand());

            await _roleService.DidNotReceiveWithAnyArgs().RevokeRoleAsync(default, default, default);
            await _dbContext.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
        }

        [Theory]
        [TestCase(-1)]
        [TestCase(-60)]
        [TestCase(-60 * 24)]
        public async Task ShouldRemoveDbEntries_WhenMutesThatAreExpiredExist(int secondsInFuture)
        {
            var now = DateTime.Now;
            _dateTime.Now.Returns(now);

            var activeMute = GenerateActiveMute(now.AddSeconds(secondsInFuture));
            var activeMutes = _dbContext.ConfigureMockDbSet(x => x.ModerationActiveMutes, activeMute);

            List<ActiveMute> removedRestrictions = null;
            activeMutes.When(x => x.RemoveRange(Arg.Any<IEnumerable<ActiveMute>>())).Do(x => removedRestrictions = x.Arg<IEnumerable<ActiveMute>>().ToList());

            await _appFixture.SendAsync(new UnMuteExpiredMutesCommand());

            removedRestrictions.Should().NotBeNullOrEmpty();
            removedRestrictions.Should().HaveCount(1);
            var restriction = removedRestrictions.First();
            restriction.id.Should().Be(activeMute.id);

            await _dbContext.ReceivedWithAnyArgs(1).SaveChangesAsync(default);
        }

        [Theory]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-60)]
        [TestCase(-60 * 24)]
        public async Task ShouldRevokeRole_WhenMutesThatAreExpiredExist(int secondsInFuture)
        {
            var now = DateTime.Now;
            _dateTime.Now.Returns(now);

            var activeMute = GenerateActiveMute(now.AddSeconds(secondsInFuture));
            _dbContext.ConfigureMockDbSet(x => x.ModerationActiveMutes, activeMute);

            await _appFixture.SendAsync(new UnMuteExpiredMutesCommand());

            await _roleService.ReceivedWithAnyArgs(1).RevokeRoleAsync(default, default, default);
            await _roleService.Received(1).RevokeRoleAsync(null, _roleId, activeMute.uId, "mute expired");
        }

        [Theory]
        [TestCase(new[] { -60, -60, -60 })]
        [TestCase(new[] { -60, 90, 500, 900, -404 })]
        public async Task ShouldRevokeRoles_WhenMultipleMutesAreExpired(int[] secondsFromNowArray)
        {
            var now = DateTime.Now;
            _dateTime.Now.Returns(now);

            var mutes = secondsFromNowArray.Select(x => GenerateActiveMute(now.AddSeconds(x))).ToList();
            _dbContext.ConfigureMockDbSet(x => x.ModerationActiveMutes, mutes);

            await _appFixture.SendAsync(new UnMuteExpiredMutesCommand());

            await _roleService.ReceivedWithAnyArgs(secondsFromNowArray.Count(x => x <= 0)).RevokeRoleAsync(default, default, default);

            foreach (var restrict in mutes.Where(x => x.expirationDate <= now.ToUniversalTime()))
            {
                await _roleService.Received(1).RevokeRoleAsync(null, _roleId, restrict.uId, "mute expired");
            }
        }

        private static ActiveMute GenerateActiveMute(DateTime expirationDateTime)
        {
            var faker = new Faker<ActiveMute>()
                    .RuleFor(x => x.id, f => f.UniqueIndex)
                    .RuleFor(x => x.uId, f => f.Random.ULong(500, 90000))
                    .RuleFor(x => x.expirationDate, expirationDateTime.ToUniversalTime())
                ;
            return faker.Generate();
        }
    }
}
