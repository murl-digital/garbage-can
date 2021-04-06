using System.Collections.Generic;
using System.Linq;
using GarbageCan.Web.Models;
using GarbageCan.XP.Boosters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Web.Controllers
{
    [ApiController]
    [Route("boosters")]
    public class BoostersController
    {
        [HttpGet]
        [SwaggerOperation("Gets all current active boosters")]
        public List<Booster> Get()
        {
            return BoosterManager.activeBoosters.Select(b => new Booster
            {
                Multiplier = b.multiplier,
                Expiration = b.expirationDate
            }).ToList();
        }

        [HttpGet]
        [Route("queue")]
        [SwaggerOperation("Gets all boosters that are queued and waiting for an available slot")]
        public List<QueuedBooster> GetQueuedBoosters()
        {
            return BoosterManager.queuedBoosters.Select(b => new QueuedBooster
            {
                Multiplier = b.multiplier,
                DurationInSeconds = b.durationInSeconds
            }).ToList();
        }
    }
}