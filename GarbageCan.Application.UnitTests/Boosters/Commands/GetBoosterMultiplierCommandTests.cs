using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using GarbageCan.Application.Boosters.Commands;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Boosters;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Boosters.Commands
{
    public class GetBoosterMultiplierCommandTests
    {
        private ApplicationFixture _appFixture;
        private IBoosterService _boosterService;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _boosterService = Substitute.For<IBoosterService>();

            _appFixture.OnConfigureServices += (_, collection) => { collection.AddSingleton(_boosterService); };
        }

        [Test]
        public async Task ShouldReturnMultiplier_IfActiveBoosterExists(
            [Range(0.1f, 68f /*lol*/, 0.1f)] float multiplier)
        {
            ulong guildId = 6;
            var booster = new ActiveBooster
            {
                Multiplier = multiplier
            };

            var activeBoosters = new Dictionary<ulong, List<ActiveBooster>>
            {
                { guildId, new List<ActiveBooster> { booster } }
            };

            _boosterService.ActiveBoosters.Returns(activeBoosters);

            var command = new GetBoosterMultiplierCommand
            {
                GuildId = guildId
            };

            var result = await _appFixture.SendAsync(command);

            result.Should().Be(1 + multiplier);
        }

        [Test]
        public async Task ShouldReturnMultiplier_IfMultipleActiveBoostersExist(
            [Range(0.1f, 5f /*lol*/, 0.1f)] float multiplier, [Random(0.1f, 5f, 1)] float otherMultiplier)
        {
            ulong guildId = 6;
            var booster = new ActiveBooster
            {
                Multiplier = multiplier
            };
            var otherBooster = new ActiveBooster
            {
                Multiplier = otherMultiplier
            };

            var activeBoosters = new Dictionary<ulong, List<ActiveBooster>>
            {
                { guildId, new List<ActiveBooster> { booster, otherBooster } }
            };

            _boosterService.ActiveBoosters.Returns(activeBoosters);

            var command = new GetBoosterMultiplierCommand
            {
                GuildId = guildId
            };

            var result = await _appFixture.SendAsync(command);

            result.Should().Be(1 + multiplier + otherMultiplier);
        }

        [Test]
        public async Task ShouldReturnOne_IfNoActiveBoostersExist()
        {
            ulong guildId = 6;

            var activeBoosters = new Dictionary<ulong, List<ActiveBooster>>
            {
                { guildId, new List<ActiveBooster>() }
            };

            _boosterService.ActiveBoosters.Returns(activeBoosters);

            var command = new GetBoosterMultiplierCommand
            {
                GuildId = guildId
            };

            var result = await _appFixture.SendAsync(command);

            result.Should().Be(1);
        }

        [Test]
        public async Task ShouldReturnOne_IfGuildDoesNotExist()
        {
            ulong guildId = 6;

            var activeBoosters = new Dictionary<ulong, List<ActiveBooster>>
            {
                { guildId + 1, new List<ActiveBooster>() }
            };

            _boosterService.ActiveBoosters.Returns(activeBoosters);

            var command = new GetBoosterMultiplierCommand
            {
                GuildId = guildId
            };

            var result = await _appFixture.SendAsync(command);

            result.Should().Be(1);
        }
    }
}
