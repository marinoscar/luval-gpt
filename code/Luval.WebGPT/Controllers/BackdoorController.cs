using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using Luval.GPT.Utilities;
using Luval.WebGPT.Data.ViewModel;
using Luval.WebGPT.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luval.WebGPT.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    [TokenFilter]
    public class BackdoorController : Controller
    {

        private readonly ILogger _logger;
        private readonly IRepository _repository;
        public BackdoorController(ILogger logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAgents([FromBody] List<PushAgent> agents)
        {

            if (agents == null || !agents.Any())
                return BadRequest("Payload is not in the correct format");


            var userId = Convert.ToString(this.ControllerContext.HttpContext.Request.RouteValues["UserId"]);
            var user = _repository.GetApplicationUser(userId);
            foreach (var agent in agents)
            {
                agent.AppUserId = user.Id;
                agent.UpdatedBy = user.Id;
                agent.UtcUpdatedOn = DateTime.UtcNow;
            }
            var res = await _repository.UpdateOrCreatePushAgent(agents);
            return Ok(res);
        }
    }
}
