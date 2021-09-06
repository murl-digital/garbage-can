using System.Threading.Tasks;
using FluentAssertions;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.LevelRoles.Commands.RemoveLevelRole;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Roles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class RemoveLevelRoleCommandTests
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
        public async Task ShouldRemoveLevelRole_WhenLevelRolesExists()
        {
            const int id = 5;

            LevelRole role = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.LevelRoles, new LevelRole {Id = id});
            mockDbSet.When(x => x.Remove(Arg.Any<LevelRole>())).Do(x => role = x.Arg<LevelRole>());

            await _appFixture.SendAsync(new RemoveLevelRoleCommand
            {
                Id = id
            });

            await _dbContext.Received(1).SaveChangesAsync(default);
            _dbContext.LevelRoles.Received(1).Remove(Arg.Any<LevelRole>());

            role.Should().NotBeNull();
            role.Id.Should().Be(id);
        }

        [Test]
        public void ShouldThrowNotFoundException_WhenLevelRolesDoesNotExist()
        {
            _dbContext.ConfigureMockDbSet(x => x.LevelRoles);

            var command = new RemoveLevelRoleCommand
            {
                Id = 50
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().Throw<NotFoundException>()
                .WithMessage("Couldn't find Level Role");
        }

        [Theory]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void ShouldThrowValidationException_WhenIdIsInvalid(int roleId)
        {
            var command = new RemoveLevelRoleCommand
            {
                Id = roleId
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().Throw<ValidationException>();
        }
    }
}
