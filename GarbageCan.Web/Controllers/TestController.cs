using GarbageCan.Application.XP.Queries.GetTopUsersByXP;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GarbageCan.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<GetTopUsersByXPQueryVm> Get()
        {
            return await _mediator.Send(new GetTopUsersByXPQuery { Count = 10, CurrentUserId = 80 });
        }
    }
}