using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luval.WebGPT.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> CreateAgents(IEnumerable<PushAgent> agents)
        {
            var res = await _repository.UpdateOrCreatePushAgent(agents);
            return Ok(res);
        }
    }
}
