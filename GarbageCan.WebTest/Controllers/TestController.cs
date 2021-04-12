using GarbageCan.Application.XP.Queries.GetTopUsersByXP;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GarbageCan.WebTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IMediator _mediator;

        public TestController(ILogger<TestController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<GetTopUsersByXPQueryVm> Get()
        {
            return await _mediator.Send(new GetTopUsersByXPQuery { Count = 10, CurrentUserId = 80 });
        }
    }
}