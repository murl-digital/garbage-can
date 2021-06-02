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

            var keySet = conditionalRoles.Keys;

            var taskList = new List<Task>();

            foreach (var member in request.Members)
            {
                foreach (var role in member.Value.Intersect(keySet)
                    .Where(r => !member.Value.Intersect(conditionalRoles[r]).Any()))
                {
                    // if (conditionalRolesEntity.First(r => r.resultRoleId == role.Id).remain) continue;
                    await _roleService.RevokeRoleAsync(request.GuildId, role, member.Key, "conditional roles");
                }

                foreach (var (resultingRole, _) in conditionalRoles
                    .Where(r => !member.Value.Contains(r.Key))
                    .Where(r => member.Value.Intersect(r.Value).Any()))
                {
                    await _roleService.GrantRoleAsync(request.GuildId, resultingRole, member.Key, "conditional roles");
                }
            }

            await Task.WhenAll(taskList);

            return await Task.FromResult(Unit.Value);
        }
    }
}