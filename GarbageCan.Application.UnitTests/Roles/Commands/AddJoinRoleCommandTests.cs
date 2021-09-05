using System.Threading.Tasks;
using FluentAssertions;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.JoinRoles.Commands.AddJoinRole;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Roles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class AddJoinRoleCommandTests
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
        public async Task ShouldAddJoinRole_WhenNoJoinRolesExist()
        {
            ulong roleId = 5;

            JoinRole addedRole = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.JoinRoles);
            mockDbSet.When(x => x.AddAsync(Arg.Any<JoinRole>())).Do(x => addedRole = x.Arg<JoinRole>());

            await _appFixture.SendAsync(new AddJoinRoleCommand
            {
                RoleId = roleId
            });

            await _dbContext.Received(1).SaveChangesAsync(default);
            await _dbContext.JoinRoles.Received(1).AddAsync(Arg.Any<JoinRole>());

            addedRole.Should().NotBeNull();
            addedRole.RoleId.Should().Be(roleId);
        }

        [Theory]
        [TestCase(0u)]
        public void ShouldThrowValidationException_WhenRoleIdIsInvalid(ulong roleId)
        {
            var command = new AddJoinRoleCommand
            {
                RoleId = roleId
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().Throw<ValidationException>();
        }
    }
}
