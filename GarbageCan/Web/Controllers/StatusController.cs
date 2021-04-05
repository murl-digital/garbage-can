using System.Threading.Tasks;
using GarbageCan.Data;
using GarbageCan.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GarbageCan.Web.Controllers
{
    [ApiController]
    [Route("status")]
    public class StatusController
    {
        [HttpGet]
        public Status Get()
        {
            return new()
            {
                Ping = GarbageCan.Client.Ping,
            };
        }
    }
}