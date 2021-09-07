using System.Threading.Tasks;
using FluentAssertions;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.ConditionalRoles.Commands.RemoveConditionalRole;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Roles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class RemoveConditionalRoleCommandTests
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
        public async Task ShouldRemoveConditionalRole_WhenConditionalRolesExists()
        {
            const int id = 5;

            ConditionalRole role = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.ConditionalRoles, new ConditionalRole {Id = id});
            mockDbSet.When(x => x.Remove(Arg.Any<ConditionalRole>())).Do(x => role = x.Arg<ConditionalRole>());

            await _appFixture.SendAsync(new RemoveConditionalRoleCommand
            {
                Id = id
            });

            await _dbContext.Received(1).SaveChangesAsync(default);
            _dbContext.ConditionalRoles.Received(1).Remove(Arg.Any<ConditionalRole>());

            role.Should().NotBeNull();
            role.Id.Should().Be(id);
        }

        [Test]
        public void ShouldThrowNotFoundException_WhenConditionalRolesDoesNotExist()
        {
            _dbContext.ConfigureMockDbSet(x => x.ConditionalRoles);

            var command = new RemoveConditionalRoleCommand
            {
                Id = 50
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().ThrowAsync<NotFoundException>()
                .WithMessage("Couldn't find Conditional Role");
        }

        [Theory]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void ShouldThrowValidationException_WhenIdIsInvalid(int roleId)
        {
            var command = new RemoveConditionalRoleCommand
            {
                Id = roleId
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().ThrowAsync<ValidationException>();
        }
    }
}
