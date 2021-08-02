using System.Collections.Generic;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.Commands.ApplyConiditionalRoles;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Roles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class ApplyConditionalRoleCommandTests
    {
        private ApplicationFixture _appFixture;
        private IApplicationDbContext _dbContext;
        private IDiscordGuildRoleService _roleService;

        [SetUp]
        public void Setup()
        {
            _appFixture = new ApplicationFixture();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _roleService = Substitute.For<IDiscordGuildRoleService>();
            _appFixture.OnConfigureServices += (_, services) =>
            {
                services.AddSingleton(_dbContext);
                services.AddSingleton(_roleService);
            };
        }

        [Test]
        public async Task ShouldGrantResultingRoleToMember_WhenMemberHasRequiredRole()
        {
            ulong requiredRoleId = 5;
            ulong resultingRoleId = 553;

            _dbContext.ConfigureMockDbSet(x => x.ConditionalRoles, new ConditionalRole
            {
                Id = 0,
                Remain = false,
                RequiredRoleId = requiredRoleId,
                ResultRoleId = resultingRoleId
            });

            await _appFixture.SendAsync(new ApplyConditionalRolesCommand
            {
                GuildId = 1,
                Members = new Dictionary<ulong, ulong[]>
                {
                    {69, new[] {requiredRoleId}}
                }
            });

            await _roleService.Received().GrantRoleAsync(1, resultingRoleId, 69, Arg.Any<string>());
        }

        [Test]
        public async Task ShouldRevokeResultingRoleFromMember_WhenMemberNoLongerHasRequiredRole()
        {
            ulong requiredRoleId = 5;
            ulong resultingRoleId = 553;
            ulong userId = 69;

            _dbContext.ConfigureMockDbSet(x => x.ConditionalRoles, new ConditionalRole
            {
                Id = 0,
                Remain = false,
                RequiredRoleId = requiredRoleId,
                ResultRoleId = resultingRoleId
            });

            await _appFixture.SendAsync(new ApplyConditionalRolesCommand
            {
                GuildId = 1,
                Members = new Dictionary<ulong, ulong[]>
                {
                    {userId, new[] {resultingRoleId}}
                }
            });

            await _roleService.Received().RevokeRoleAsync(1, resultingRoleId, userId, Arg.Any<string>());
        }

        [Test]
        public async Task ShouldNotRevokeResultingRoleFromMember_WhenConditionalRoleIsMarkedAsRemain()
        {
            ulong requiredRoleId = 5;
            ulong resultingRoleId = 553;
            ulong userId = 69;

            _dbContext.ConfigureMockDbSet(x => x.ConditionalRoles, new ConditionalRole
            {
                Id = 0,
                Remain = true,
                RequiredRoleId = requiredRoleId,
                ResultRoleId = resultingRoleId
            });

            await _appFixture.SendAsync(new ApplyConditionalRolesCommand
            {
                GuildId = 1,
                Members = new Dictionary<ulong, ulong[]>
                {
                    {userId, new[] {resultingRoleId}}
                }
            });

            await _roleService.DidNotReceiveWithAnyArgs().RevokeRoleAsync(default, default, default);
        }

        [Test]
        public async Task ShouldDoNothing_WhenMemberAlreadySatisfiesConditions()
        {
            ulong requiredRoleId = 5;
            ulong resultingRoleId = 553;
            ulong userId = 69;

            _dbContext.ConfigureMockDbSet(x => x.ConditionalRoles, new ConditionalRole
            {
                Id = 0,
                Remain = false,
                RequiredRoleId = requiredRoleId,
                ResultRoleId = resultingRoleId
            });

            await _appFixture.SendAsync(new ApplyConditionalRolesCommand
            {
                GuildId = 1,
                Members = new Dictionary<ulong, ulong[]>
                {
                    {userId, new[] {requiredRoleId, resultingRoleId}}
                }
            });

            await _roleService.DidNotReceiveWithAnyArgs().RevokeRoleAsync(default, default, default);
            await _roleService.DidNotReceiveWithAnyArgs().GrantRoleAsync(default, default, default);
        }

        [Test]
        public async Task ShouldDoNothing_WhenNoConditionalRolesExist()
        {
            ulong requiredRoleId = 5;
            ulong resultingRoleId = 553;
            ulong userId = 69;


            _dbContext.ConfigureMockDbSet(x => x.ConditionalRoles);

            await _appFixture.SendAsync(new ApplyConditionalRolesCommand
            {
                GuildId = 1,
                Members = new Dictionary<ulong, ulong[]>
                {
                    {userId, new[] {requiredRoleId, resultingRoleId}}
                }
            });

            await _roleService.DidNotReceiveWithAnyArgs().RevokeRoleAsync(default, default, default);
            await _roleService.DidNotReceiveWithAnyArgs().GrantRoleAsync(default, default, default);
        }
    }
}
