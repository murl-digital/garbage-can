using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Common.Models;
using GarbageCan.Application.XP.EventHandlers;
using GarbageCan.Domain.Events;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.XP.EventHandlers
{
    public class UserLevelUpEventUserBoosterHandlerTests
    {
        private IDiscordWebhookService _mediator;
        private UserLevelUpEventEvilTwinHandler _sut;
        private const ulong GuildId = 50;
        private const ulong UserId = 5;
        private const ulong ChannelId = 534;

        [SetUp]
        public void Setup()
        {
            _mediator = Substitute.For<IDiscordWebhookService>();

            _sut = new UserLevelUpEventEvilTwinHandler(_mediator, new NullLogger<UserLevelUpEventEvilTwinHandler>());
        }

        [Test]
        public async Task Handle_ShouldSendWithWebhookMessage_AsEvilClone()
        {
            string displayName = "TEST";
            string AvatarUrl = "AVATAR";

            var newLvl = 10;
            await _sut.Handle(new DomainEventNotification<UserLevelUpEvent>(new UserLevelUpEvent
            {
                MessageDetails = new MessageDetails
                {
                    GuildId = GuildId,
                    UserId = UserId,
                    UserAvatarUrl = AvatarUrl,
                    UserDisplayName = displayName, 
                    ChannelId = ChannelId
                },
                OldLvl = 5,
                NewLvl = newLvl,
            }), CancellationToken.None);

            await _mediator.Received(1)
                .CreateUserWebhook("Level up!",
                    $"Congrats to {displayName} for reaching level {newLvl}!",
                    $"{displayName}'s evil clone",
                    AvatarUrl,
                    GuildId,
                    ChannelId);
        }
    }
}
