using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarbageCan.Application.Boosters.ActiveBoosters.Queries;
using GarbageCan.Application.Boosters.QueuedBoosters.Queries;
using GarbageCan.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Controllers
{
    [ApiController]
    [Route("boosters")]
    public class BoostersController
    {
        private readonly IMediator _mediator;

        public BoostersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [SwaggerOperation("Gets all current active boosters")]
        public async Task<List<Booster>> Get()
        {
            var boosters = await _mediator.Send(new GetAllActiveBoostersQuery());
            return boosters.Select(b => new Booster
            {
                Multiplier = b.Multiplier,
                Expiration = b.ExpirationDate
            }).ToList();
        }

        [HttpGet]
        [Route("queue")]
        [SwaggerOperation("Gets all boosters that are queued and waiting for an available slot")]
        public async Task<List<QueuedBooster>> GetQueuedBoosters()
        {
            var boosters = await _mediator.Send(new GetAllQueuedBoostersQuery());
            return boosters.Select(b => new QueuedBooster
            {
                Multiplier = b.Multiplier,
                DurationInSeconds = b.DurationInSeconds
            }).ToList();
        }
    }
}
