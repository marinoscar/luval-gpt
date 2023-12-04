using Microsoft.AspNetCore.Mvc;

namespace Luval.WebGPT.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult Index()
        {
            return Json("Worked");
        }
    }
}
