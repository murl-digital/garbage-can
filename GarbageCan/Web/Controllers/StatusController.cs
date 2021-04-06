using System.Threading.Tasks;
using GarbageCan.Data;
using GarbageCan.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Web.Controllers
{
    [ApiController]
    [Route("status")]
    public class StatusController
    {
        [HttpGet]
        [SwaggerOperation("Gets the bot's current status")]
        public Status Get()
        {
            return new()
            {
                Ping = GarbageCan.Client.Ping,
            };
        }
    }
}