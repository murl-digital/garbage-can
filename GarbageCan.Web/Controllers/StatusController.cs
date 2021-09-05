using DSharpPlus;
using GarbageCan.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Web.Controllers
{
    [ApiController]
    [Route("status")]
    public class StatusController
    {
        private readonly DiscordClient _client;

        public StatusController(DiscordClient client)
        {
            _client = client;
        }

        [HttpGet]
        [SwaggerOperation("Gets the bot's current status")]
        public Status Get()
        {
            return new()
            {
                Ping = _client.Ping,
            };
        }
    }
}
