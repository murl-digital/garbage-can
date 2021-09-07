using System.Threading.Tasks;
using FluentAssertions;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.JoinRoles.Commands.RemoveJoinRole;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Roles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class RemoveJoinRoleCommandTests
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
        public async Task ShouldRemoveJoinRole_WhenJoinRolesExists()
        {
            const int id = 5;

            JoinRole role = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.JoinRoles, new JoinRole {Id = id});
            mockDbSet.When(x => x.Remove(Arg.Any<JoinRole>())).Do(x => role = x.Arg<JoinRole>());

            await _appFixture.SendAsync(new RemoveJoinRoleCommand
            {
                Id = id
            });

            await _dbContext.Received(1).SaveChangesAsync(default);
            _dbContext.JoinRoles.Received(1).Remove(Arg.Any<JoinRole>());

            role.Should().NotBeNull();
            role.Id.Should().Be(id);
        }

        [Test]
        public void ShouldThrowNotFoundException_WhenJoinRolesDoesNotExist()
        {
            _dbContext.ConfigureMockDbSet(x => x.JoinRoles);

            var command = new RemoveJoinRoleCommand
            {
                Id = 50
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().ThrowAsync<NotFoundException>()
                .WithMessage("Couldn't find Join Role");
        }

        [Theory]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void ShouldThrowValidationException_WhenIdIsInvalid(int roleId)
        {
            var command = new RemoveJoinRoleCommand
            {
                Id = roleId
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().ThrowAsync<ValidationException>();
        }
    }
}
