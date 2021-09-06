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
    public class ActivateBoosterCommandTests
    {
        private ApplicationFixture _appFixture;
        private IApplicationDbContext _dbContext;
        private IBoosterService _boosterService;
        private IDateTime _dateTime;
        private IDiscordGuildChannelService _discordChannelService;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _boosterService = Substitute.For<IBoosterService>();
            _dateTime = Substitute.For<IDateTime>();
            _discordChannelService = Substitute.For<IDiscordGuildChannelService>();

            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_dbContext);
                services.AddSingleton(_boosterService);
                services.AddSingleton(_dateTime);
                services.AddSingleton(_discordChannelService);
            };
        }

        [Test]
        public async Task ShouldActivateBooster_WhenSlotExists()
        {
            var now = DateTime.Now;
            ulong guildId = 6;
            ulong channelId = 7;
            var multiplier = 0.5f;
            var duration = TimeSpan.FromSeconds(5);
            var slot = new AvailableSlot
            {
                Id = 69,
                GuildId = guildId,
                ChannelId = channelId
            };

            var activeBoosters = new Dictionary<ulong, List<ActiveBooster>>
            {
                { guildId, new List<ActiveBooster>() }
            };
            var availableSlots = new Dictionary<ulong, List<AvailableSlot>>
            {
                { guildId, new List<AvailableSlot> { slot } }
            };

            _dbContext.ConfigureMockDbSet(x => x.XPActiveBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPAvailableSlots, slot);
            _boosterService.ActiveBoosters.Returns(activeBoosters);
            _boosterService.AvailableSlots.Returns(availableSlots);

            _dateTime.Now.Returns(now);

            var command = new ActivateBoosterCommand
            {
                GuildId = guildId,
                Multiplier = multiplier,
                Duration = duration,
                Slot = slot
            };

            await _appFixture.SendAsync(command);

            await _dbContext.Received(1).SaveChangesAsync(default);
            await _discordChannelService.Received(1).RenameChannel(guildId, slot.ChannelId, Arg.Any<string>());

            _dbContext.XPActiveBoosters.Received(1).Add(Arg.Any<ActiveBooster>());
            var booster = activeBoosters[guildId].First();
            booster.GuildId.Should().Be(guildId);
            booster.Slot.Id.Should().Be(slot.Id);
            booster.Multiplier.Should().Be(multiplier);
            booster.ExpirationDate.Should().Be(now.ToUniversalTime().Add(duration));
        }

        [Test]
        public async Task ShouldThrowException_WhenGuildHasNoSlots()
        {
            ulong guildId = 6;
            ulong channelId = 7;
            var multiplier = 0.5f;
            var duration = TimeSpan.FromSeconds(5);
            var slot = new AvailableSlot
            {
                Id = 69,
                GuildId = guildId,
                ChannelId = channelId
            };

            var availableSlots = new Dictionary<ulong, List<AvailableSlot>>();

            _boosterService.AvailableSlots.Returns(availableSlots);


            var command = new ActivateBoosterCommand
            {
                GuildId = guildId,
                Multiplier = multiplier,
                Duration = duration,
                Slot = slot
            };

            Func<Task> act = async () => await _appFixture.SendAsync(command);

            act.Should().Throw<InvalidOperationException>().WithMessage("Specified guild has no available slots");
        }

        [Test]
        public async Task ShouldThrowException_WhenGuildDoesntHaveSpecifiedSlots()
        {
            ulong guildId = 6;
            ulong channelId = 7;
            var multiplier = 0.5f;
            var duration = TimeSpan.FromSeconds(5);
            var slot = new AvailableSlot
            {
                Id = 69,
                GuildId = guildId,
                ChannelId = channelId
            };
            var wrongSlot = new AvailableSlot
            {
                Id = 70,
                GuildId = guildId,
                ChannelId = channelId
            };

            var availableSlots = new Dictionary<ulong, List<AvailableSlot>>
            {
                { guildId, new List<AvailableSlot> { slot } }
            };

            _boosterService.AvailableSlots.Returns(availableSlots);


            var command = new ActivateBoosterCommand
            {
                GuildId = guildId,
                Multiplier = multiplier,
                Duration = duration,
                Slot = wrongSlot
            };

            Func<Task> act = async () => await _appFixture.SendAsync(command);

            act.Should().Throw<InvalidOperationException>().WithMessage("Specified slot doesn't exist in guild");
        }
    }
}
