using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luval.WebGPT.Controllers
{
    [Authorize]
    public class BackdoorController : Controller
    {
        [HttpPost]
        public IActionResult CreateAgents()
        {
            return View();
        }
    }
}
