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
            ulong guildId = 16;
            ulong resultingRoleId = 553;
            ulong memberId = 69;
            var level = 5;

            _dbContext.ConfigureMockDbSet(x => x.LevelRoles, new LevelRole
            {
                Id = 0,
                Remain = false,
                Lvl = level,
                RoleId = resultingRoleId,
                GuildId = guildId
            });

            await _appFixture.SendAsync(new ApplyLevelRolesCommand
            {
                GuildId = guildId,
                Level = level,
                MemberId = memberId,
                RoleIds = Array.Empty<ulong>()
            });

            await _roleService.Received().GrantRoleAsync(guildId, resultingRoleId, memberId, Arg.Any<string>());
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
            ulong guildId = 16;
            ulong resultingRoleId = 553;
            ulong userId = 69;

            _dbContext.ConfigureMockDbSet(x => x.LevelRoles, new[]
            {
                new LevelRole
                {
                    Id = 0,
                    Remain = false,
                    Lvl = 4,
                    RoleId = resultingRoleId,
                    GuildId = guildId
                },
                new LevelRole
                {
                    Id = 1,
                    Remain = false,
                    Lvl = 6,
                    RoleId = resultingRoleId + 1,
                    GuildId = guildId
                }
            });

            await _appFixture.SendAsync(new ApplyLevelRolesCommand
            {
                GuildId = guildId,
                MemberId = userId,
                Level = level,
                RoleIds = new[] {resultingRoleId}
            });

            await _roleService.Received().RevokeRoleAsync(guildId, resultingRoleId, userId, Arg.Any<string>());
            await _roleService.Received().GrantRoleAsync(guildId, resultingRoleId + 1, userId, Arg.Any<string>());
        }
    }
}
