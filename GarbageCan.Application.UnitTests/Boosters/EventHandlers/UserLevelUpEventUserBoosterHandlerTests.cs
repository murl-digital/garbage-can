using System;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Boosters.EventHandlers;
using GarbageCan.Application.Boosters.UserBoosters.Commands;
using GarbageCan.Application.Common.Models;
using GarbageCan.Domain.Events;
using MediatR;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Boosters.EventHandlers
{
    public class UserLevelUpEventUserBoosterHandlerTests
    {
        private IMediator _mediator;
        private UserLevelUpEventUserBoosterHandler _sut;
        private const ulong GuildId = 50;
        private const ulong UserId = 5;

        [SetUp]
        public void Setup()
        {
            _mediator = Substitute.For<IMediator>();

            _sut = new UserLevelUpEventUserBoosterHandler(_mediator);
        }

        [Test]
        [TestCase(0, 1)]
        [TestCase(4, 5)]
        [TestCase(3, 9)]
        public async Task Handle_ShouldNotCreateUserBooster_WhenLevelGreaterThan10(int oldLevel, int newLevel)
        {
            await RunHandler(oldLevel, newLevel);

            await _mediator.DidNotReceiveWithAnyArgs().Send(Arg.Any<AddUserBoosterCommand>(), Arg.Any<CancellationToken>());
        }

        [Test]
        [TestCase(10, 11)]
        [TestCase(10, 14)]
        [TestCase(11, 14)]
        public async Task Handle_ShouldNotCreateUserBooster_WhenLevelIsNotAMultipleOf5(int oldLevel, int newLevel)
        {
            await RunHandler(oldLevel, newLevel);
            await _mediator.DidNotReceiveWithAnyArgs().Send(Arg.Any<AddUserBoosterCommand>(), Arg.Any<CancellationToken>());
        }

        [Test]
        [TestCase(9, 10)]
        [TestCase(14, 15)]
        [TestCase(11, 19)]
        public async Task Handle_ShouldCreateUserBooster_WhenLevelIsAMultipleOf5OrHasCrossedAMultipleOf5(int oldLevel, int newLevel)
        {
            await RunHandler(oldLevel, newLevel);

            await _mediator.Received(1).Send(
                Arg.Is<AddUserBoosterCommand>(command =>
                    command.GuildId == GuildId &&
                    command.UserId == UserId &&
                    command.Duration == TimeSpan.FromMinutes(30) &&
                    Math.Abs(command.Multiplier - 1.5f) < 0.0001
                ),
                Arg.Any<CancellationToken>());
        }

        [Test]
        [TestCase(9, 16, 2)]
        [TestCase(9, 20, 3)]
        [TestCase(4, 100, 19)]
        public async Task Handle_ShouldCreateMultipleUserBoosters_WhenLevelSurpassesMultipleMultiplesOf5(int oldLevel, int newLevel, int numberOfBoosters)
        {
            await RunHandler(oldLevel, newLevel);

            await _mediator.Received(numberOfBoosters).Send(
                Arg.Is<AddUserBoosterCommand>(command =>
                    command.GuildId == GuildId &&
                    command.UserId == UserId &&
                    command.Duration == TimeSpan.FromMinutes(30) &&
                    Math.Abs(command.Multiplier - 1.5f) < 0.0001
                ),
                Arg.Any<CancellationToken>());
        }

        private async Task RunHandler(int oldLevel, int newLevel)
        {
            await _sut.Handle(new DomainEventNotification<UserLevelUpEvent>(new UserLevelUpEvent
            {
                MessageDetails = new MessageDetails
                {
                    GuildId = GuildId,
                    UserId = UserId
                },
                OldLvl = oldLevel,
                NewLvl = newLevel,
            }), CancellationToken.None);
        }
    }
}
