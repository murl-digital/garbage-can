using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Models;
using GarbageCan.Application.Roles.LevelRoles.Commands.ApplyLevelRoles;
using GarbageCan.Application.Roles.LevelRoles.EventHandlers;
using GarbageCan.Domain.Events;
using MediatR;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Roles.EventHandlers
{
    public class UserLevelUpEventHandlerTests
    {
        private IMediator _mediator;
        private UserLevelUpEventHandler _sut;
        private const ulong GuildId = 50;
        private const ulong UserId = 5;

        [SetUp]
        public void Setup()
        {
            _mediator = Substitute.For<IMediator>();

            _sut = new UserLevelUpEventHandler(_mediator);
        }


        [Test]
        [TestCase(9, 10, new[] { 10 })]
        [TestCase(14, 15, new[] { 15 })]
        [TestCase(11, 19, new[] { 12, 13, 14, 15, 16, 17, 18, 19 })]
        public async Task Handle_ShouldApplyLevelRoles_WhenForLevelChange(int oldLevel, int newLevel, int[] levels)
        {
            await RunHandler(oldLevel, newLevel);
            foreach (var level in levels)
            {
                await _mediator.Received(1).Send(
                    Arg.Is<ApplyLevelRolesCommand>(command =>
                        command.GuildId == GuildId &&
                        command.MemberId == UserId &&
                        command.Level == level
                    ),
                    Arg.Any<CancellationToken>());
            }
        }

        private async Task RunHandler(int oldLevel, int newLevel)
        {
            await _sut.Handle(new DomainEventNotification<UserLevelUpEvent>(new UserLevelUpEvent
            {
                GuildId = GuildId,
                UserId = UserId,
                OldLvl = oldLevel,
                NewLvl = newLevel,
            }), CancellationToken.None);
        }
    }
}
