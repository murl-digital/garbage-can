using Bogus;
using FluentAssertions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Moderation.Commands.Restrict;
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
    public class UnRestrictExpiredChannelsCommandTests
    {
        private readonly ulong _guildId = 151;
        private ApplicationFixture _appFixture;
        private IDateTime _dateTime;
        private IApplicationDbContext _dbContext;
        private IDiscordConfiguration _discordConfiguration;
        private IDiscordModerationService _moderationService;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _dateTime = Substitute.For<IDateTime>();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _moderationService = Substitute.For<IDiscordModerationService>();
            _discordConfiguration = Substitute.For<IDiscordConfiguration>();

            _discordConfiguration.GuildId.Returns(_guildId);

            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_dbContext);
                services.AddSingleton(_dateTime);
                services.AddSingleton(_moderationService);
                services.AddSingleton(_discordConfiguration);
            };
        }

        [Theory]
        [TestCase(1)]
        [TestCase(60)]
        [TestCase(60 * 24)]
        public async Task ShouldNotRestoreAnyChannels_WhenChannelRestrictionsThatAreNotExpiredExist(int secondsInFuture)
        {
            var now = DateTime.Now;
            _dateTime.Now.Returns(now);

            var activeRestriction = GenerateActiveChannelRestrict(now.AddSeconds(secondsInFuture));
            _dbContext.ConfigureMockDbSet(x => x.ModerationActiveChannelRestricts, activeRestriction);

            await _appFixture.SendAsync(new UnRestrictExpiredChannelsCommand());

            await _moderationService.DidNotReceiveWithAnyArgs().RestoreChannelAccess(default, default, default);
            await _dbContext.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
        }

        [Test]
        public async Task ShouldNotRestoreAnyChannels_WhenNoChannelRestrictionsExist()
        {
            _dbContext.ConfigureMockDbSet(x => x.ModerationActiveChannelRestricts);

            await _appFixture.SendAsync(new UnRestrictExpiredChannelsCommand());

            await _moderationService.DidNotReceiveWithAnyArgs().RestoreChannelAccess(default, default, default);
            await _dbContext.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
        }

        [Theory]
        [TestCase(-1)]
        [TestCase(-60)]
        [TestCase(-60 * 24)]
        public async Task ShouldRemoveDbEntries_WhenChannelRestrictionsThatAreExpiredExist(int secondsInFuture)
        {
            var now = DateTime.Now;
            _dateTime.Now.Returns(now);

            var activeRestriction = GenerateActiveChannelRestrict(now.AddSeconds(secondsInFuture));
            var activeChannelRestricts = _dbContext.ConfigureMockDbSet(x => x.ModerationActiveChannelRestricts, activeRestriction);

            List<ActiveChannelRestrict> removedRestrictions = null;
            activeChannelRestricts.When(x => x.RemoveRange(Arg.Any<IEnumerable<ActiveChannelRestrict>>())).Do(x => removedRestrictions = x.Arg<IEnumerable<ActiveChannelRestrict>>().ToList());

            await _appFixture.SendAsync(new UnRestrictExpiredChannelsCommand());

            removedRestrictions.Should().NotBeNullOrEmpty();
            removedRestrictions.Should().HaveCount(1);
            var restriction = removedRestrictions.First();
            restriction.id.Should().Be(activeRestriction.id);

            await _dbContext.ReceivedWithAnyArgs(1).SaveChangesAsync(default);
        }

        [Theory]
        [TestCase(new[] { -60, -60, -60 })]
        [TestCase(new[] { -60, 90, 500, 900, -404 })]
        public async Task ShouldRestoreAllChannels_WhenMultipleChannelRestrictionsAreExpired(int[] secondsFromNowArray)
        {
            var now = DateTime.Now;
            _dateTime.Now.Returns(now);

            var channelRestricts = secondsFromNowArray.Select(x => GenerateActiveChannelRestrict(now.AddSeconds(x))).ToList();
            _dbContext.ConfigureMockDbSet(x => x.ModerationActiveChannelRestricts, channelRestricts);

            await _appFixture.SendAsync(new UnRestrictExpiredChannelsCommand());

            await _moderationService.ReceivedWithAnyArgs(secondsFromNowArray.Count(x => x <= 0)).RestoreChannelAccess(default, default, default);

            foreach (var restrict in channelRestricts.Where(x => x.expirationDate <= now))
            {
                await _moderationService.Received(1).RestoreChannelAccess(_guildId, restrict.uId, restrict.channelId, "channel restrict expired");
            }
        }

        [Theory]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-60)]
        [TestCase(-60 * 24)]
        public async Task ShouldRestoreChannels_WhenChannelRestrictionsThatAreExpiredExist(int secondsInFuture)
        {
            var now = DateTime.Now;
            _dateTime.Now.Returns(now);

            var activeRestriction = GenerateActiveChannelRestrict(now.AddSeconds(secondsInFuture));
            _dbContext.ConfigureMockDbSet(x => x.ModerationActiveChannelRestricts, activeRestriction);

            await _appFixture.SendAsync(new UnRestrictExpiredChannelsCommand());

            await _moderationService.ReceivedWithAnyArgs(1).RestoreChannelAccess(default, default, default);
            await _moderationService.Received(1).RestoreChannelAccess(_guildId, activeRestriction.uId, activeRestriction.channelId, "channel restrict expired");
        }

        private static ActiveChannelRestrict GenerateActiveChannelRestrict(DateTime expirationDateTime)
        {
            var faker = new Faker<ActiveChannelRestrict>()
                    .RuleFor(x => x.id, f => f.UniqueIndex)
                    .RuleFor(x => x.uId, f => f.Random.ULong(500, 90000))
                    .RuleFor(x => x.channelId, f => f.Random.ULong(2000, 700000))
                    .RuleFor(x => x.expirationDate, expirationDateTime.ToUniversalTime())
                ;
            return faker.Generate();
        }
    }
}