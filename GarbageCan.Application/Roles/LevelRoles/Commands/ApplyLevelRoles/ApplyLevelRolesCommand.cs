using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.Commands.ApplyLevelRoles
{
    public class ApplyLevelRolesCommand : IRequest
    {
        public ulong GuildId { get; set; }
        public ulong MemberId { get; set; }
        public int Level { get; set; }
        public ulong[] RoleIds { get; set; }
    }

    public class ApplyLevelRolesCommandHandler : IRequestHandler<ApplyLevelRolesCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordGuildRoleService _roleService;

        public ApplyLevelRolesCommandHandler(IApplicationDbContext context, IDiscordGuildRoleService roleService)
        {
            _context = context;
            _roleService = roleService;
        }

        public async Task<Unit> Handle(ApplyLevelRolesCommand request, CancellationToken cancellationToken)
        {
            var levelRoles = await _context.LevelRoles
                .Where(r => r.GuildId == request.GuildId)
                .Where(r => r.Lvl <= request.Level)
                .ToArrayAsync(cancellationToken);

            foreach (var roleId in levelRoles
                .Where(r => r.Lvl == request.Level)
                .Select(r => r.RoleId))
                await _roleService.GrantRoleAsync(request.GuildId, roleId, request.MemberId, "level roles");

            var groups = levelRoles
                .Where(r => !r.Remain)
                .GroupBy(r => r.Lvl)
                .OrderBy(r => r.Key)
                .ToDictionary(k => k.Key, v => v.Select(r => r.RoleId).ToArray());
            var keySet = groups.Keys.ToArray();

            for (var i = 0; i < keySet.Length - 1; i++)
            {
                if (request.Level >= keySet[i] && request.Level < keySet[i + 1]) continue;

                foreach (var role in groups[keySet[i]].Intersect(request.RoleIds))
                    await _roleService.RevokeRoleAsync(request.GuildId, role, request.MemberId, "level roles");
            }

            return Unit.Value;
        }
    }
}
