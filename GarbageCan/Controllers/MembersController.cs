using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarbageCan.Application.Members.Queries;
using GarbageCan.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GarbageCan.Controllers
{
    [ApiController]
    [Route("members")]
    public class MembersController : Controller
    {
        private readonly IMediator _mediator;

        public MembersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [SwaggerOperation("Gets all members in the server (excluding bot accounts)",
            "This endpoint is paginated with a default size of 10 and a maximum size of 20")]
        public async Task<List<Member>> Get([FromQuery] PaginationFilter filter)
        {
            var memberVms = await _mediator.Send(new GetMembersQuery
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            });
            
            return memberVms.Select(x => new Member
            {
                Id = x.Id.ToString(),
                Level = x.Level,
                Name = x.Name,
                Xp = x.Xp,
            }).ToList();
        }


        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation("Gets a member with a specific id, passed in the url")]
        [SwaggerResponse(200, type: typeof(Member))]
        [SwaggerResponse(404, "Requested member not found")]
        [SwaggerResponse(400, "Requested member is a bot account")]
        public async Task<IActionResult> Get(ulong id)
        {
            var member = await _mediator.Send(new GetMemberByIdQuery { Id = id });

            if (member == null) return NotFound();
            if (member.IsBot) return BadRequest();

            return Ok(new Member
            {
                Id = id.ToString(),
                Name = member.Name,
                Xp = member.Xp,
                Level = member.Level
            });
        }
    }
}
