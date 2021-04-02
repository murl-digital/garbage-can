using System.Collections.Generic;
using System.Threading.Tasks;
using GarbageCan.Data;
using GarbageCan.Web.Models;
using GarbageCan.XP;
using Microsoft.AspNetCore.Mvc;

namespace GarbageCan.Web.Controllers
{
    [ApiController]
    [Route("members")]
    public class MembersController : Controller
    {
        [HttpGet]
        public async Task<List<Member>> Get()
        {
            var result = new List<Member>();
            await using var context = new Context();
            var members = await GarbageCan.Client.Guilds[GarbageCan.OperatingGuildId].GetAllMembersAsync();

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