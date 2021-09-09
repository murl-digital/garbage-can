using System.Threading.Tasks;
using GarbageCan.Application.Fun.Commands.RandomStatus;
using MediatR;
using Quartz;

namespace GarbageCan.Jobs
{
    public class RandomizeStatusJob : IJob
    {
        private readonly IMediator _mediator;

        public RandomizeStatusJob(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _mediator.Send(new RandomizeStatusCommand());
        }
    }
}
