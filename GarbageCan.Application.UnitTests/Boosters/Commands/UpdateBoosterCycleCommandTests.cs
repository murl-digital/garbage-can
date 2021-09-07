using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GarbageCan.Application.Boosters.Commands;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Boosters;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using Z.EntityFramework.Plus;

namespace GarbageCan.Application.UnitTests.Boosters.Commands
{
    public class UpdateBoosterCycleCommandTests
    {
        // ApplicationFixture _appFixture;
        private UpdateBoosterCycleCommandHandler _sussy;
        private IMediator _mediator;
        private IApplicationDbContext _dbContext;
        private IBoosterService _boosterService;
        private IDateTime _dateTime;
        private IDiscordGuildChannelService _discordChannelService;

        [SetUp]
        public void Setup()
        {
            _mediator = Substitute.For<IMediator>();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _boosterService = Substitute.For<IBoosterService>();
            _dateTime = Substitute.For<IDateTime>();
            _discordChannelService = Substitute.For<IDiscordGuildChannelService>();

            _sussy = new UpdateBoosterCycleCommandHandler(_dbContext, _boosterService, _dateTime,
                _discordChannelService, _mediator);
        }

        [Theory]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(7)]
        public async Task ShouldDoNothing_IfNoBoostersAreActive(int numSlots)
        {
            const ulong guildId = 6;
            const ulong channelId = 7;
            var slots = new List<AvailableSlot>();
            for (var i = 0; i < numSlots; i++)
                slots.Add(new AvailableSlot
                {
                    GuildId = guildId,
                    ChannelId = channelId + (ulong)numSlots
                });

            var activeBoosters = new Dictionary<ulong, List<ActiveBooster>>
            {
                { guildId, Substitute.For<List<ActiveBooster>>() }
            };
            var queuedBoosters = new Dictionary<ulong, Queue<QueuedBooster>>
            {
                { guildId, new Queue<QueuedBooster>() }
            };
            var availableSlots = new Dictionary<ulong, List<AvailableSlot>>
            {
                {
                    guildId, slots
                }
            };
            _boosterService.ActiveBoosters.Returns(activeBoosters);
            _boosterService.QueuedBoosters.Returns(queuedBoosters);
            _boosterService.AvailableSlots.Returns(availableSlots);

            _dbContext.ConfigureMockDbSet(x => x.XPActiveBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPQueuedBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPAvailableSlots);

            // z.efplus deleteasync(), a blessing and a curse 
            ((IQueryable<QueuedBooster>)_dbContext.XPQueuedBoosters).Expression.Returns(new List<QueuedBooster>()
                .AsQueryable().Where(x => false).Expression);
            ((IQueryable<ActiveBooster>)_dbContext.XPActiveBoosters).Expression.Returns(new List<ActiveBooster>()
                .AsQueryable().Where(x => false).Expression);

            _dateTime.Now.Returns(DateTime.Now);

            await _sussy.Handle(new UpdateBoosterCycleCommand { GuildId = guildId }, CancellationToken.None);

            //await _dbContext.XPActiveBoosters.DidNotReceive().DeleteAsync();
            _boosterService.ActiveBoosters[guildId].DidNotReceiveWithAnyArgs().RemoveAll(_ => false);
        }

        [Test]
        public async Task ShouldDeactivateBoosters_IfBoostersAreExpired()
        {
            ulong guildId = 6;
            ulong channelId = 7;
            var now = DateTime.Now;
            var slot = new AvailableSlot
            {
                GuildId = guildId,
                ChannelId = channelId
            };

            var sub = new List<ActiveBooster>
            {
                new()
                {
                    GuildId = guildId,
                    ExpirationDate = now.ToUniversalTime().Subtract(TimeSpan.FromSeconds(5)),
                    Multiplier = 6.9f,
                    Slot = slot
                }
            };
            var activeBoosters = new Dictionary<ulong, List<ActiveBooster>>
            {
                { guildId, sub }
            };
            var queuedBoosters = new Dictionary<ulong, Queue<QueuedBooster>>
            {
                { guildId, new Queue<QueuedBooster>() }
            };
            var availableSlots = new Dictionary<ulong, List<AvailableSlot>>
            {
                {
                    guildId, new List<AvailableSlot>
                    {
                        slot
                    }
                }
            };
            _boosterService.ActiveBoosters.Returns(activeBoosters);
            _boosterService.QueuedBoosters.Returns(queuedBoosters);
            _boosterService.AvailableSlots.Returns(availableSlots);

            _dbContext.ConfigureMockDbSet(x => x.XPActiveBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPQueuedBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPAvailableSlots);

            ((IQueryable<QueuedBooster>)_dbContext.XPQueuedBoosters).Expression.Returns(new List<QueuedBooster>()
                .AsQueryable().Where(x => false).Expression);
            ((IQueryable<ActiveBooster>)_dbContext.XPActiveBoosters).Expression.Returns(new List<ActiveBooster>()
                .AsQueryable().Where(x => false).Expression);

            _dateTime.Now.Returns(now);

            await _sussy.Handle(new UpdateBoosterCycleCommand { GuildId = guildId }, CancellationToken.None);

            await _dbContext.XPActiveBoosters.Received().DeleteAsync();
            _boosterService.ActiveBoosters[guildId].Count.Should().Be(0);

            await _discordChannelService.Received().RenameChannel(guildId, channelId, "-");
        }

        [Test]
        public async Task ShouldActivateQueuedBooster_IfSlotBecomesAvailable()
        {
            ulong guildId = 6;
            ulong channelId = 7;
            var now = DateTime.Now.ToUniversalTime();
            var slot = new AvailableSlot
            {
                GuildId = guildId,
                ChannelId = channelId
            };
            var sub = new List<ActiveBooster>
            {
                new()
                {
                    GuildId = guildId,
                    ExpirationDate = now.Subtract(TimeSpan.FromSeconds(5)),
                    Multiplier = 6.9f,
                    Slot = slot
                }
            };
            var queued = new QueuedBooster
            {
                GuildId = guildId,
                Multiplier = 5.0f,
                DurationInSeconds = 600
            };

            var activeBoosters = new Dictionary<ulong, List<ActiveBooster>>
            {
                { guildId, sub }
            };
            var queuedBoosters = new Dictionary<ulong, Queue<QueuedBooster>>
            {
                { guildId, new Queue<QueuedBooster>(new[] { queued }) }
            };
            var availableSlots = new Dictionary<ulong, List<AvailableSlot>>
            {
                {
                    guildId, new List<AvailableSlot>
                    {
                        slot
                    }
                }
            };
            _boosterService.ActiveBoosters.Returns(activeBoosters);
            _boosterService.QueuedBoosters.Returns(queuedBoosters);
            _boosterService.AvailableSlots.Returns(availableSlots);

            _dbContext.ConfigureMockDbSet(x => x.XPActiveBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPQueuedBoosters);
            _dbContext.ConfigureMockDbSet(x => x.XPAvailableSlots);

            ((IQueryable<QueuedBooster>)_dbContext.XPQueuedBoosters).Expression.Returns(new List<QueuedBooster>()
                .AsQueryable().Where(x => false).Expression);
            ((IQueryable<ActiveBooster>)_dbContext.XPActiveBoosters).Expression.Returns(new List<ActiveBooster>()
                .AsQueryable().Where(x => false).Expression);

            _dateTime.Now.Returns(now);

            await _sussy.Handle(new UpdateBoosterCycleCommand { GuildId = guildId }, CancellationToken.None);

            await _dbContext.XPActiveBoosters.Received().DeleteAsync();
            _boosterService.ActiveBoosters[guildId].Count.Should().Be(0);

            await _mediator.Received().Send(Arg.Is<ActivateBoosterCommand>(c =>
                c.GuildId == guildId &&
                Math.Abs(c.Multiplier - queued.Multiplier) < 0.01f &&
                c.Duration == TimeSpan.FromSeconds(queued.DurationInSeconds)
            ));
            await _mediator.Received(1).Send(Arg.Is<SaveQueueCommand>(c =>
                c.GuildId == guildId
            ));
        }
    }
}
