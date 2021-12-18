using System.Threading.Tasks;
using GarbageCan.Application.Roles.JoinRoles.Commands.ApplyJoinRoles;
using MediatR;
using Quartz;

namespace GarbageCan.Jobs
{
    public class ApplyJoinRolesJob : IJob
    {
        private readonly IMediator _mediator;

        public ApplyJoinRolesJob(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _mediator.Send(new ApplyJoinRolesCommand());
        }
    }
}
