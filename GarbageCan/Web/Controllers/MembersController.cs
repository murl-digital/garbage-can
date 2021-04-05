using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarbageCan.Web.Filters;
using GarbageCan.Web.Models;
using GarbageCan.XP;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Web.Controllers
{
    [ApiController]
    [Route("members")]
    public class MembersController : Controller
    {
        [HttpGet]
        [SwaggerOperation("Gets all members in the server (excluding bot accounts)", "This endpoint is paginated with a default size of 10 and a maximum size of 20")]
        public async Task<List<Member>> Get([FromQuery] PaginationFilter filter)
        {
            var ( pageNumber, pageSize ) = new PaginationFilter(filter.PageNumber, filter.PageSize);
            
            var result = new List<Member>();
            var members = GarbageCan.Client.Guilds[GarbageCan.OperatingGuildId].Members
                .Where(m => !m.Value.IsBot)
                .Select(m => m.Value)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArray();

            foreach (var member in members)
            {
                if (member.IsBot) continue;
                var xpMember = await XpManager.GetUser(member.Id);
                result.Add(new Member
                {
                    Id = member.Id,
                    Name = member.DisplayName,
                    Xp = xpMember.xp,
                    Level = xpMember.lvl
                });
            }

            return result;
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation("Gets a member with a specific id, passed in the url")]
        [SwaggerResponse(200, type: typeof(Member))]
        [SwaggerResponse(404, "Requested member not found")]
        [SwaggerResponse(400, "Requested member is a bot account")]
        public async Task<IActionResult> Get(ulong id)
        {
            var member = await GarbageCan.Client.Guilds[GarbageCan.OperatingGuildId].GetMemberAsync(id);
            if (member == null) return NotFound();
            if (member.IsBot) return BadRequest();
            var user = await XpManager.GetUser(id);

            return Ok(new Member
            {
                Id = id,
                Name = member.DisplayName,
                Xp = user.xp,
                Level = user.lvl
            });
        }
    }
}