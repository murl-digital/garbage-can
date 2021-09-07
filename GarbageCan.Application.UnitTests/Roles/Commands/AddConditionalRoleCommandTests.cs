using System.Threading.Tasks;
using FluentAssertions;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.ConditionalRoles.Commands.AddConditionalRole;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Roles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class AddConditionalRoleCommandTests
    {
        private ApplicationFixture _appFixture;
        private IApplicationDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_dbContext);
            };
        }

        [Test]
        public async Task ShouldAddConditionalRole_WhenNoConditionalRolesExist()
        {
            ulong requiredRoleId = 5;
            ulong resultingRoleId = 553;

            ConditionalRole addedRole = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.ConditionalRoles);
            mockDbSet.When(x => x.AddAsync(Arg.Any<ConditionalRole>())).Do(x => addedRole = x.Arg<ConditionalRole>());

            await _appFixture.SendAsync(new AddConditionalRoleCommand
            {
                RequiredRoleId = requiredRoleId,
                ResultRoleId = resultingRoleId,
                Remain = true
            });

            await _dbContext.Received(1).SaveChangesAsync(default);
            await _dbContext.ConditionalRoles.Received(1).AddAsync(Arg.Any<ConditionalRole>());

            addedRole.Should().NotBeNull();
            addedRole.RequiredRoleId.Should().Be(requiredRoleId);
            addedRole.ResultRoleId.Should().Be(resultingRoleId);
            addedRole.Remain.Should().BeTrue();
        }

        [Theory]
        [TestCase(0u, 50u)]
        [TestCase(90u, 0u)]
        [TestCase(0u, 0u)]
        public void ShouldThrowValidationException_WhenRoleIdIsInvalid(ulong requestRoleId, ulong resultRoleId)
        {
            var command = new AddConditionalRoleCommand
            {
                ResultRoleId = resultRoleId,
                RequiredRoleId = requestRoleId
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().ThrowAsync<ValidationException>();
        }
    }
}
