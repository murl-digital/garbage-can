using System.Collections.Generic;
using System.Linq;
using GarbageCan.Web.Models;
using GarbageCan.XP.Boosters;
using Microsoft.AspNetCore.Mvc;

namespace GarbageCan.Web.Controllers
{
    [ApiController]
    [Route("boosters")]
    public class BoostersController
    {
        [HttpGet]
        public List<Booster> Get()
        {
            return BoosterManager.activeBoosters.Select(b => new Booster
            {
                multiplier = b.multiplier,
                expiration = b.expirationDate
            }).ToList();
        }

        [HttpGet]
        [Route("queue")]
        public List<QueuedBooster> GetQueuedBoosters()
        {
            return BoosterManager.queuedBoosters.Select(b => new QueuedBooster
            {
                multiplier = b.multiplier,
                durationInSeconds = b.durationInSeconds
            }).ToList();
        }
    }
}