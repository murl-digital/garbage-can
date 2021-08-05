using System.Threading.Tasks;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.Roles.ConditionalRoles.Commands.ApplyConditionalRoles;
using MediatR;
using Quartz;

namespace GarbageCan.Web.Jobs
{
    public class ApplyConditionalRolesJob : IJob
    {
        private readonly IMediator _mediator;
        private readonly IDiscordGuildRoleService _roleService;

        public ApplyConditionalRolesJob(IMediator mediator, IDiscordGuildRoleService roleService)
        {
            _mediator = mediator;
            _roleService = roleService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            foreach (var (guildId, membersWithRoles) in await _roleService.GetAllMembersAndRoles())
            {
                await _mediator.Send(new ApplyConditionalRolesCommand
                {
                    GuildId = guildId,
                    Members = membersWithRoles
                });
            }
        }
    }
}