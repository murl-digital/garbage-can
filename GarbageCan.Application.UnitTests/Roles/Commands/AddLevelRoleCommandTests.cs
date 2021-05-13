using FluentAssertions;
using GarbageCan.Application.Common.Exceptions;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.Commands.AddLevelRole;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Roles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class AddLevelRoleCommandTests
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
        public async Task ShouldAddLevelRole_WhenNoLevelRolesExist()
        {
            ulong roleId = 5;
            int level = 45;

            LevelRole addedRole = null;
            var mockDbSet = _dbContext.ConfigureMockDbSet(x => x.LevelRoles);
            mockDbSet.When(x => x.AddAsync(Arg.Any<LevelRole>())).Do(x => addedRole = x.Arg<LevelRole>());

            await _appFixture.SendAsync(new AddLevelRoleCommand
            {
                RoleId = roleId,
                Level = level,
                Remain = true
            });

            await _dbContext.Received(1).SaveChangesAsync(default);
            await _dbContext.LevelRoles.Received(1).AddAsync(Arg.Any<LevelRole>());

            addedRole.Should().NotBeNull();
            addedRole.roleId.Should().Be(roleId);
            addedRole.lvl.Should().Be(level);
            addedRole.remain.Should().BeTrue();
        }

        [Test]
        public async Task ShouldRespondWithMessage_WhenAddedSuccessfully()
        {
            _dbContext.ConfigureMockDbSet(x => x.LevelRoles);

            await _appFixture.SendAsync(new AddLevelRoleCommand
            {
                RoleId = 5,
                Level = 50
            });

            await _responseService.Received(1).RespondAsync("Role added successfully", true);
            await _responseService.Received(1).RespondAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>());
        }

        [Theory]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void ShouldThrowValidationException_WhenLevelIsInvalid(int level)
        {
            var command = new AddLevelRoleCommand
            {
                RoleId = 906,
                Level = level
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Theory]
        [TestCase(0u)]
        public void ShouldThrowValidationException_WhenRoleIdIsInvalid(ulong roleId)
        {
            var command = new AddLevelRoleCommand
            {
                Level = 906,
                RoleId = roleId
            };

            FluentActions.Invoking(() => _appFixture.SendAsync(command)).Should().Throw<ValidationException>();
        }
    }
}