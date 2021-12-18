using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GarbageCan.Application.Roles.JoinRoles.Commands.ApplyJoinRoles
{
    public class ApplyJoinRolesCommand : IRequest { }

    public class ApplyJoinRolesCommandHandler : IRequestHandler<ApplyJoinRolesCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IDiscordGuildService _discordGuildService;
        private readonly IDiscordGuildRoleService _discordGuildRoleService;
        private readonly IDiscordGuildMemberService _discordGuildMemberService;
        private readonly ILogger<ApplyJoinRolesCommandHandler> _logger;

        public ApplyJoinRolesCommandHandler(IApplicationDbContext applicationDbContext,
            IDiscordGuildService discordGuildService, IDiscordGuildRoleService discordGuildRoleService,
            IDiscordGuildMemberService discordGuildMemberService, ILogger<ApplyJoinRolesCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext;
            _discordGuildService = discordGuildService;
            _discordGuildRoleService = discordGuildRoleService;
            _discordGuildMemberService = discordGuildMemberService;
            _logger = logger;
        }

        public async Task<Unit> Handle(ApplyJoinRolesCommand request, CancellationToken cancellationToken)
        {
            foreach (var guildId in _discordGuildService.GetAllCurrentGuildIds())
            {
                var joinRoles = await _applicationDbContext.JoinRoles
                    .Where(r => r.GuildId == guildId)
                    .ToListAsync(cancellationToken);

                foreach (var guildMember in _discordGuildMemberService.GetGuildMembers(guildId))
                {
                    if (guildMember.IsPending)
                        continue;

                    foreach (var r in joinRoles)
                        try
                        {
                            await _discordGuildRoleService.GrantRoleAsync(guildId,
                                r.RoleId,
                                guildMember.Id,
                                "join role");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "couldn't grant role to user");
                        }
                }
            }

            return Unit.Value;
        }
    }
}
