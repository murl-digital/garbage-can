using System;
using System.Collections.Generic;
using System.Linq;
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
    public class PopulateBoosterServiceCommandTests
    {
        private ApplicationFixture _appFixture;
        private IApplicationDbContext _dbContext;
        private IBoosterService _boosterService;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _boosterService = Substitute.For<IBoosterService>();

            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_dbContext);
                services.AddSingleton(_boosterService);
            };
        }

        [Test]
        public async Task AllCollectionsShouldBeEmpty_WhenSlotDoesNotExist()
        {
            Dictionary<ulong, List<ActiveBooster>> activeBoosters = null;
            Dictionary<ulong, Queue<QueuedBooster>> queuedBoosters = null;
            Dictionary<ulong, List<AvailableSlot>> availableSlots = null;
            _boosterService.ActiveBoosters = Arg.Do<Dictionary<ulong, List<ActiveBooster>>>(x => activeBoosters = x);
            _boosterService.QueuedBoosters = Arg.Do<Dictionary<ulong, Queue<QueuedBooster>>>(x => queuedBoosters = x);
            _boosterService.AvailableSlots = Arg.Do<Dictionary<ulong, List<AvailableSlot>>>(x => availableSlots = x);
            _boosterService.ActiveBoosters.Returns(activeBoosters);
            _boosterService.QueuedBoosters.Returns(queuedBoosters);
            _boosterService.AvailableSlots.Returns(availableSlots);

            _dbContext.ConfigureMockDbSet(x => x.XPActiveBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPQueuedBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPAvailableSlots);

            await _appFixture.SendAsync(new PopulateBoosterServiceCommand());

            activeBoosters.Should().NotBeNull().And.BeEmpty();
            queuedBoosters.Should().NotBeNull().And.BeEmpty();
            availableSlots.Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public async Task OtherCollectionsShouldNotBeEmpty_WhenSlotExists()
        {
            ulong guildId = 6;
            ulong channelId = 7;
            var slot = new AvailableSlot
            {
                Id = 69,
                GuildId = guildId,
                ChannelId = channelId
            };

            Dictionary<ulong, List<ActiveBooster>> activeBoosters = null;
            Dictionary<ulong, Queue<QueuedBooster>> queuedBoosters = null;
            Dictionary<ulong, List<AvailableSlot>> availableSlots = null;
            _boosterService.ActiveBoosters = Arg.Do<Dictionary<ulong, List<ActiveBooster>>>(x => activeBoosters = x);
            _boosterService.QueuedBoosters = Arg.Do<Dictionary<ulong, Queue<QueuedBooster>>>(x => queuedBoosters = x);
            _boosterService.AvailableSlots = Arg.Do<Dictionary<ulong, List<AvailableSlot>>>(x => availableSlots = x);
            _boosterService.ActiveBoosters.Returns(activeBoosters);
            _boosterService.QueuedBoosters.Returns(queuedBoosters);
            _boosterService.AvailableSlots.Returns(availableSlots);

            _dbContext.ConfigureMockDbSet(x => x.XPActiveBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPQueuedBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPAvailableSlots, slot);

            await _appFixture.SendAsync(new PopulateBoosterServiceCommand());

            activeBoosters.Should().NotBeNull();
            queuedBoosters.Should().NotBeNull();
            availableSlots.Should().NotBeNull();

            availableSlots[guildId].First().GuildId.Should().Be(guildId);
            availableSlots[guildId].First().ChannelId.Should().Be(channelId);
            activeBoosters[guildId].Should().NotBeNull().And.BeEmpty();
            queuedBoosters[guildId].Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public async Task QueueShouldBeInCorrectOrder_WhenPopulatedFromDatabase([Range(2u, 50, 1)] uint count)
        {
            ulong guildId = 6;
            ulong channelId = 7;
            var slot = new AvailableSlot
            {
                Id = 69,
                GuildId = guildId,
                ChannelId = channelId
            };
            var boosters = new List<QueuedBooster>();
            var random = new Random();
            for (uint i = 0; i < count; i++)
                boosters.Add(new QueuedBooster
                {
                    Id = (int)i,
                    GuildId = guildId,
                    Position = i,
                    Multiplier = (float)random.NextDouble() * 3f,
                    DurationInSeconds = random.Next(5, 500)
                });

            Dictionary<ulong, List<ActiveBooster>> activeBoosters = null;
            Dictionary<ulong, Queue<QueuedBooster>> queuedBoosters = null;
            Dictionary<ulong, List<AvailableSlot>> availableSlots = null;
            _boosterService.ActiveBoosters = Arg.Do<Dictionary<ulong, List<ActiveBooster>>>(x => activeBoosters = x);
            _boosterService.QueuedBoosters = Arg.Do<Dictionary<ulong, Queue<QueuedBooster>>>(x => queuedBoosters = x);
            _boosterService.AvailableSlots = Arg.Do<Dictionary<ulong, List<AvailableSlot>>>(x => availableSlots = x);
            _boosterService.ActiveBoosters.Returns(activeBoosters);
            _boosterService.QueuedBoosters.Returns(queuedBoosters);
            _boosterService.AvailableSlots.Returns(availableSlots);

            _dbContext.ConfigureMockDbSet(x => x.XPActiveBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPQueuedBoosters, boosters);
            _dbContext.ConfigureMockDbSet(x => x.XPAvailableSlots, slot);

            await _appFixture.SendAsync(new PopulateBoosterServiceCommand());

            activeBoosters.Should().NotBeNull();
            queuedBoosters.Should().NotBeNull();
            availableSlots.Should().NotBeNull();

            queuedBoosters[guildId].Count.Should().Be(boosters.Count);

            foreach (var ogBooster in boosters)
            {
                var compareAgainstBooster = queuedBoosters[guildId].Dequeue();

                CompareQueuedBoosters(ogBooster, compareAgainstBooster);
            }
        }

        [Test]
        public async Task QueueShouldBeInCorrectOrder_WhenMultipleGuildsArePresent([Range(20u, 30, 1)] uint count)
        {
            ulong guildId = 6;
            ulong otherGuildId = 7;
            ulong channelId = 7;
            var slot = new AvailableSlot
            {
                Id = 69,
                GuildId = guildId,
                ChannelId = channelId
            };
            var otherSlot = new AvailableSlot
            {
                Id = 69,
                GuildId = guildId,
                ChannelId = channelId
            };
            var boosters = new List<QueuedBooster>();
            var otherBoosters = new List<QueuedBooster>();
            var random = new Random();
            for (uint i = 0; i < count; i++)
                boosters.Add(new QueuedBooster
                {
                    Id = (int)i,
                    GuildId = guildId,
                    Position = i,
                    Multiplier = (float)random.NextDouble() * 3f,
                    DurationInSeconds = random.Next(5, 500)
                });
            for (uint i = 0; i < count + random.Next(-10, 10); i++)
                otherBoosters.Add(new QueuedBooster
                {
                    Id = (int)i,
                    GuildId = otherGuildId,
                    Position = i,
                    Multiplier = (float)random.NextDouble() * 3f,
                    DurationInSeconds = random.Next(5, 500)
                });

            Dictionary<ulong, List<ActiveBooster>> activeBoosters = null;
            Dictionary<ulong, Queue<QueuedBooster>> queuedBoosters = null;
            Dictionary<ulong, List<AvailableSlot>> availableSlots = null;
            _boosterService.ActiveBoosters = Arg.Do<Dictionary<ulong, List<ActiveBooster>>>(x => activeBoosters = x);
            _boosterService.QueuedBoosters = Arg.Do<Dictionary<ulong, Queue<QueuedBooster>>>(x => queuedBoosters = x);
            _boosterService.AvailableSlots = Arg.Do<Dictionary<ulong, List<AvailableSlot>>>(x => availableSlots = x);
            _boosterService.ActiveBoosters.Returns(activeBoosters);
            _boosterService.QueuedBoosters.Returns(queuedBoosters);
            _boosterService.AvailableSlots.Returns(availableSlots);

            _dbContext.ConfigureMockDbSet(x => x.XPActiveBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPQueuedBoosters, boosters.Union(otherBoosters));
            _dbContext.ConfigureMockDbSet(x => x.XPAvailableSlots, new[] { slot, otherSlot });

            await _appFixture.SendAsync(new PopulateBoosterServiceCommand());

            activeBoosters.Should().NotBeNull();
            queuedBoosters.Should().NotBeNull();
            availableSlots.Should().NotBeNull();

            queuedBoosters[guildId].Count.Should().Be(boosters.Count);
            queuedBoosters[otherGuildId].Count.Should().Be(otherBoosters.Count);

            foreach (var ogBooster in boosters)
            {
                var compareAgainstBooster = queuedBoosters[guildId].Dequeue();

                CompareQueuedBoosters(ogBooster, compareAgainstBooster);
            }

            foreach (var ogBooster in otherBoosters)
            {
                var compareAgainstBooster = queuedBoosters[otherGuildId].Dequeue();

                CompareQueuedBoosters(ogBooster, compareAgainstBooster);
            }
        }

        [Test]
        public async Task ActiveBoostersShouldBeCorrect_WhenPopulatedFromDatabase()
        {
            ulong guildId = 6;
            ulong channelId = 7;
            ulong otherChannelId = 8;
            var multiplier = 0.5f;
            var expirationDate = DateTime.Now.ToUniversalTime().AddMinutes(5);
            var slot = new AvailableSlot
            {
                Id = 69,
                GuildId = guildId,
                ChannelId = channelId
            };
            var otherSlot = new AvailableSlot
            {
                Id = 70,
                GuildId = guildId,
                ChannelId = otherChannelId
            };

            var booster = new ActiveBooster
            {
                GuildId = guildId,
                Slot = slot,
                Multiplier = multiplier,
                ExpirationDate = expirationDate
            };

            Dictionary<ulong, List<ActiveBooster>> activeBoosters = null;
            Dictionary<ulong, Queue<QueuedBooster>> queuedBoosters = null;
            Dictionary<ulong, List<AvailableSlot>> availableSlots = null;
            _boosterService.ActiveBoosters = Arg.Do<Dictionary<ulong, List<ActiveBooster>>>(x => activeBoosters = x);
            _boosterService.QueuedBoosters = Arg.Do<Dictionary<ulong, Queue<QueuedBooster>>>(x => queuedBoosters = x);
            _boosterService.AvailableSlots = Arg.Do<Dictionary<ulong, List<AvailableSlot>>>(x => availableSlots = x);
            _boosterService.ActiveBoosters.Returns(activeBoosters);
            _boosterService.QueuedBoosters.Returns(queuedBoosters);
            _boosterService.AvailableSlots.Returns(availableSlots);

            _dbContext.ConfigureMockDbSet(x => x.XPActiveBoosters, booster);
            _dbContext.ConfigureMockDbSet(x => x.XPQueuedBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPAvailableSlots, new[] { slot, otherSlot });

            await _appFixture.SendAsync(new PopulateBoosterServiceCommand());

            activeBoosters.Should().NotBeNull().And.NotBeEmpty();
            queuedBoosters.Should().NotBeNull();
            availableSlots.Should().NotBeNull();

            var boosterToCheck = activeBoosters[guildId].First();
            CheckActiveBooster(boosterToCheck, slot, guildId, multiplier, expirationDate);
        }

        [Test]
        public async Task ActiveBoostersShouldBeCorrect_WhenMultipleGuildsArePresent()
        {
            ulong guildId = 6;
            ulong otherGuildId = 7;
            ulong channelId = 7;
            ulong otherChannelId = 8;
            var multiplier = 0.5f;
            var expirationDate = DateTime.Now.ToUniversalTime().AddMinutes(5);
            var slot = new AvailableSlot
            {
                Id = 69,
                GuildId = guildId,
                ChannelId = channelId
            };
            var otherSlot = new AvailableSlot
            {
                Id = 70,
                GuildId = guildId,
                ChannelId = otherChannelId
            };
            var otherGuildSlot = new AvailableSlot
            {
                Id = 71,
                GuildId = otherGuildId,
                ChannelId = channelId
            };

            var booster = new ActiveBooster
            {
                GuildId = guildId,
                Slot = slot,
                Multiplier = multiplier,
                ExpirationDate = expirationDate
            };
            var otherBooster = new ActiveBooster
            {
                GuildId = guildId,
                Slot = otherSlot,
                Multiplier = multiplier,
                ExpirationDate = expirationDate
            };
            var otherGuildBooster = new ActiveBooster
            {
                GuildId = otherGuildId,
                Slot = otherGuildSlot,
                Multiplier = multiplier,
                ExpirationDate = expirationDate
            };

            Dictionary<ulong, List<ActiveBooster>> activeBoosters = null;
            Dictionary<ulong, Queue<QueuedBooster>> queuedBoosters = null;
            Dictionary<ulong, List<AvailableSlot>> availableSlots = null;
            _boosterService.ActiveBoosters = Arg.Do<Dictionary<ulong, List<ActiveBooster>>>(x => activeBoosters = x);
            _boosterService.QueuedBoosters = Arg.Do<Dictionary<ulong, Queue<QueuedBooster>>>(x => queuedBoosters = x);
            _boosterService.AvailableSlots = Arg.Do<Dictionary<ulong, List<AvailableSlot>>>(x => availableSlots = x);
            _boosterService.ActiveBoosters.Returns(activeBoosters);
            _boosterService.QueuedBoosters.Returns(queuedBoosters);
            _boosterService.AvailableSlots.Returns(availableSlots);

            _dbContext.ConfigureMockDbSet(x => x.XPActiveBoosters, new[] { booster, otherBooster, otherGuildBooster });
            _dbContext.ConfigureMockDbSet(x => x.XPQueuedBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPAvailableSlots, new[] { slot, otherSlot, otherGuildSlot });

            await _appFixture.SendAsync(new PopulateBoosterServiceCommand());

            activeBoosters.Should().NotBeNull().And.NotBeEmpty();
            queuedBoosters.Should().NotBeNull();
            availableSlots.Should().NotBeNull();

            var boosterToCheck = activeBoosters[guildId].First();
            CheckActiveBooster(boosterToCheck, slot, guildId, multiplier, expirationDate);

            var otherBoosterToCheck = activeBoosters[guildId].Last();
            CheckActiveBooster(otherBoosterToCheck, otherSlot, guildId, multiplier, expirationDate);

            var otherGuildBoosterToCheck = activeBoosters[otherGuildId].First();
            CheckActiveBooster(otherGuildBoosterToCheck, otherGuildSlot, otherGuildId, multiplier, expirationDate);
        }

        private static void CheckActiveBooster(ActiveBooster booster, AvailableSlot slot, ulong guildId,
            float multiplier,
            DateTime expirationDate)
        {
            booster.Slot.Id.Should().Be(slot.Id);
            booster.GuildId.Should().Be(guildId);
            booster.Multiplier.Should().Be(multiplier);
            booster.ExpirationDate.Should().Be(expirationDate);
        }

        private static void CompareQueuedBoosters(QueuedBooster ogBooster, QueuedBooster compareAgainstBooster)
        {
            compareAgainstBooster.Id.Should().Be(ogBooster.Id);
            compareAgainstBooster.Position.Should().Be(ogBooster.Position);
            compareAgainstBooster.GuildId.Should().Be(ogBooster.GuildId);
            compareAgainstBooster.Multiplier.Should().Be(ogBooster.Multiplier);
            compareAgainstBooster.DurationInSeconds.Should().Be(ogBooster.DurationInSeconds);
        }
    }
}
