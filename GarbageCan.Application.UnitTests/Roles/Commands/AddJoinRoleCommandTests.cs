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
        private IDiscordResponseService _responseService;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _responseService = Substitute.For<IDiscordResponseService>();
            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_dbContext);
                services.AddSingleton(_responseService);
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

        [Test]
        public async Task ShouldRespondWithMessage_WhenAddedSuccessfully()
        {
            _dbContext.ConfigureMockDbSet(x => x.JoinRoles);

            await _appFixture.SendAsync(new AddJoinRoleCommand
            {
                RoleId = 5
            });

            await _responseService.Received(1).RespondAsync("Role added successfully", true);
            await _responseService.Received(1).RespondAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>());
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
