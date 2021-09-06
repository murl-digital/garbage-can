using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Boosters.Commands;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Boosters;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Z.EntityFramework.Plus;

namespace GarbageCan.Application.UnitTests.Boosters.Commands
{
    public class SaveQueueCommandTests
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
        public async Task DbSetShouldBeEmpty_WhenThereIsNoBoostersInQueue()
        {
            ulong guildId = 6;

            var queuedBoosters = new Dictionary<ulong, Queue<QueuedBooster>>
            {
                { guildId, new Queue<QueuedBooster>() }
            };
            _boosterService.QueuedBoosters.Returns(queuedBoosters);

            _dbContext.ConfigureMockDbSet(x => x.XPQueuedBoosters);
            ((IQueryable<QueuedBooster>)_dbContext.XPQueuedBoosters).Expression.Returns(new List<QueuedBooster>()
                .AsQueryable().Where(x => false).Expression);

            await _appFixture.SendAsync(new SaveQueueCommand { GuildId = guildId });

            await _dbContext.XPQueuedBoosters.Received(1).DeleteAsync();
            _dbContext.XPQueuedBoosters.Received(0).Add(Arg.Any<QueuedBooster>());
        }

        [Test]
        public async Task DbSetShouldContainAccurateRepresentation_WhenQueueIsNotEmpty([Range(2u, 50, 1)] uint count)
        {
            ulong guildId = 6;
            var boosters = new Queue<QueuedBooster>();
            var random = new Random();
            for (uint i = 0; i < count; i++)
                boosters.Enqueue(new QueuedBooster
                {
                    Id = (int)i,
                    GuildId = guildId,
                    Multiplier = (float)random.NextDouble() * 3f,
                    DurationInSeconds = random.Next(5, 500)
                });

            var queuedBoosters = new Dictionary<ulong, Queue<QueuedBooster>>
            {
                { guildId, boosters }
            };
            _boosterService.QueuedBoosters.Returns(queuedBoosters);

            _dbContext.ConfigureMockDbSet(x => x.XPQueuedBoosters);
            ((IQueryable<QueuedBooster>)_dbContext.XPQueuedBoosters).Expression.Returns(new List<QueuedBooster>()
                .AsQueryable().Where(x => false).Expression);

            await _appFixture.SendAsync(new SaveQueueCommand { GuildId = guildId });

            await _dbContext.XPQueuedBoosters.Received(1).DeleteAsync();

            for (uint i = 0; i < count; i++)
            {
                var booster = boosters.Dequeue();
                _dbContext.XPQueuedBoosters.Received(1).Add(Arg.Is<QueuedBooster>(b => b.Position == i &&
                    b.GuildId == booster.GuildId &&
                    Math.Abs(b.Multiplier - booster.Multiplier) < 0.01 &&
                    b.DurationInSeconds == booster.DurationInSeconds));
            }
        }

        [Test]
        public async Task DbSetShouldContainAccurateRepresentation_WhenMultipleGuildsArePresent(
            [Range(2u, 50, 1)] uint count)
        {
            ulong guildId = 6;
            var boosters = new Queue<QueuedBooster>();
            var random = new Random();
            for (uint i = 0; i < count; i++)
                boosters.Enqueue(new QueuedBooster
                {
                    Id = (int)i,
                    GuildId = guildId,
                    Multiplier = (float)random.NextDouble() * 3f,
                    DurationInSeconds = random.Next(5, 500)
                });

            var queuedBoosters = new Dictionary<ulong, Queue<QueuedBooster>>
            {
                { guildId, boosters },
                {
                    guildId + 1, new Queue<QueuedBooster>(new[]
                    {
                        new QueuedBooster
                        {
                            Id = 69,
                            GuildId = guildId + 1,
                            Multiplier = (float)random.NextDouble() * 3f,
                            DurationInSeconds = random.Next(5, 500)
                        }
                    })
                }
            };
            _boosterService.QueuedBoosters.Returns(queuedBoosters);

            _dbContext.ConfigureMockDbSet(x => x.XPQueuedBoosters);
            ((IQueryable<QueuedBooster>)_dbContext.XPQueuedBoosters).Expression.Returns(new List<QueuedBooster>()
                .AsQueryable().Where(x => false).Expression);

            await _appFixture.SendAsync(new SaveQueueCommand { GuildId = guildId });

            await _dbContext.XPQueuedBoosters.Received(1).DeleteAsync();

            for (uint i = 0; i < count; i++)
            {
                var booster = boosters.Dequeue();
                _dbContext.XPQueuedBoosters.Received(1).Add(Arg.Is<QueuedBooster>(b => b.Position == i &&
                    b.GuildId == booster.GuildId &&
                    Math.Abs(b.Multiplier - booster.Multiplier) < 0.01 &&
                    b.DurationInSeconds == booster.DurationInSeconds));
            }

            await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
