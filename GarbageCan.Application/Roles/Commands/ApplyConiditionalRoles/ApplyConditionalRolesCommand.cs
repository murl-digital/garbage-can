using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GarbageCan.Application.Roles.Commands.ApplyConiditionalRoles
{
    public class ApplyConditionalRolesCommand : IRequest
    {
        public ulong GuildId { get; set; }
        public Dictionary<ulong, ulong[]> Members { get; set; }
    }

    public class ApplyConditionalRolesCommandHandler : IRequestHandler<ApplyConditionalRolesCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordGuildRoleService _roleService;

        public ApplyConditionalRolesCommandHandler(IApplicationDbContext context, IDiscordGuildRoleService roleService)
        {
            _context = context;
            this._roleService = roleService;
        }

        public async Task<Unit> Handle(ApplyConditionalRolesCommand request, CancellationToken cancellationToken)
        {
            var conditionalRolesEntity = await _context.ConditionalRoles.ToArrayAsync(cancellationToken);
            var conditionalRoles = conditionalRolesEntity
                .GroupBy(r => r.resultRoleId)
                .ToDictionary(r => r.Key, r => r.Select(x => x.requiredRoleId).ToList());

            var resultingRolesWithNoRemain = conditionalRolesEntity
                .Where(r => !r.remain)
                .Select(r => r.resultRoleId)
                .ToArray();

            foreach (var (memberId, roles) in request.Members)
            {
                foreach (var role in roles
                    .Intersect(resultingRolesWithNoRemain)
                    .Where(r => !roles.Intersect(conditionalRoles[r]).Any()))
                {
                    await _roleService.RevokeRoleAsync(request.GuildId, role, memberId, "conditional roles");
                }

                foreach (var (resultingRole, _) in conditionalRoles
                    .Where(r => !roles.Contains(r.Key))
                    .Where(r => roles.Intersect(r.Value).Any()))
                {
                    await _roleService.GrantRoleAsync(request.GuildId, resultingRole, memberId, "conditional roles");
                }
            }

            return Unit.Value;
        }
    }
}