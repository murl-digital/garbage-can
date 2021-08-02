using System;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.Commands.ApplyLevelRoles;
using GarbageCan.Application.UnitTests.Shared;
using GarbageCan.Domain.Entities.Roles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GarbageCan.Application.UnitTests.Roles.Commands
{
    public class ApplyLevelRoleCommandTests
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
        public async Task ShouldGrantResultingRoleToMember_WhenMemberHasRequiredLevel()
        {
            ulong resultingRoleId = 553;
            ulong memberId = 69;
            var level = 5;

            _dbContext.ConfigureMockDbSet(x => x.LevelRoles, new LevelRole
            {
                Id = 0,
                Remain = false,
                Lvl = level,
                RoleId = resultingRoleId
            });

            await _appFixture.SendAsync(new ApplyLevelRolesCommand
            {
                GuildId = 1,
                Level = level,
                MemberId = memberId,
                RoleIds = Array.Empty<ulong>()
            });

            await _roleService.Received().GrantRoleAsync(1, resultingRoleId, memberId, Arg.Any<string>());
        }

        [Test]
        public async Task ShouldNotRevokeResultingRoleFromMember_WhenLevelRoleIsMarkedAsRemain()
        {
            var level = 4;
            ulong resultingRoleId = 553;
            ulong userId = 69;

            _dbContext.ConfigureMockDbSet(x => x.LevelRoles, new LevelRole
            {
                Id = 0,
                Remain = true,
                Lvl = 4,
                RoleId = resultingRoleId
            });

            await _appFixture.SendAsync(new ApplyLevelRolesCommand
            {
                GuildId = 1,
                MemberId = userId,
                Level = level,
                RoleIds = new[] {resultingRoleId}
            });

            await _roleService.DidNotReceiveWithAnyArgs().RevokeRoleAsync(default, default, default);
        }

        [Test]
        public async Task ShouldNotRevokeResultingRoleFromMember_WhenHigherLevelRoleExists()
        {
            var level = 5;
            ulong resultingRoleId = 553;
            ulong userId = 69;

            _dbContext.ConfigureMockDbSet(x => x.LevelRoles, new[]
            {
                new LevelRole
                {
                    Id = 0,
                    Remain = false,
                    Lvl = 4,
                    RoleId = resultingRoleId
                },
                new LevelRole
                {
                    Id = 1,
                    Remain = false,
                    Lvl = 6,
                    RoleId = resultingRoleId + 1
                }
            });

            await _appFixture.SendAsync(new ApplyLevelRolesCommand
            {
                GuildId = 1,
                MemberId = userId,
                Level = level,
                RoleIds = new[] {resultingRoleId}
            });

            await _roleService.DidNotReceiveWithAnyArgs().RevokeRoleAsync(default, default, default);
        }

        [Test]
        public async Task ShouldNotRevokeResultingRoleFromMember_WhenHigherLevelRoleDoesNotExist()
        {
            var level = 15;
            ulong resultingRoleId = 553;
            ulong userId = 69;

            _dbContext.ConfigureMockDbSet(x => x.LevelRoles, new[]
            {
                new LevelRole
                {
                    Id = 0,
                    Remain = false,
                    Lvl = 4,
                    RoleId = resultingRoleId
                },
                new LevelRole
                {
                    Id = 1,
                    Remain = false,
                    Lvl = 6,
                    RoleId = 69696
                }
            });

            await _appFixture.SendAsync(new ApplyLevelRolesCommand
            {
                GuildId = 1,
                MemberId = userId,
                Level = level,
                RoleIds = new ulong[] {69696}
            });

            await _roleService.DidNotReceiveWithAnyArgs().RevokeRoleAsync(default, default, default);
        }

        [Test]
        public async Task ShouldReplaceResultingRoleFromMember_WhenHigherLevelRoleExists()
        {
            var level = 6;
            ulong resultingRoleId = 553;
            ulong userId = 69;

            _dbContext.ConfigureMockDbSet(x => x.LevelRoles, new[]
            {
                new LevelRole
                {
                    Id = 0,
                    Remain = false,
                    Lvl = 4,
                    RoleId = resultingRoleId
                },
                new LevelRole
                {
                    Id = 1,
                    Remain = false,
                    Lvl = 6,
                    RoleId = resultingRoleId + 1
                }
            });

            await _appFixture.SendAsync(new ApplyLevelRolesCommand
            {
                GuildId = 1,
                MemberId = userId,
                Level = level,
                RoleIds = new[] {resultingRoleId}
            });

            await _roleService.Received().RevokeRoleAsync(1, resultingRoleId, userId, Arg.Any<string>());
            await _roleService.Received().GrantRoleAsync(1, resultingRoleId + 1, userId, Arg.Any<string>());
        }

        /*[Test]
        public async Task ShouldDoNothing_WhenMemberAlreadySatisfiesConditions()
        {
            ulong requiredRoleId = 5;
            ulong resultingRoleId = 553;
            ulong userId = 69;

            _dbContext.ConfigureMockDbSet(x => x.ConditionalRoles, new ConditionalRole
            {
                id = 0,
                remain = false,
                requiredRoleId = requiredRoleId,
                resultRoleId = resultingRoleId
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
        }*/
    }
}
