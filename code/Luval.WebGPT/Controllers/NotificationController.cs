using Luval.GPT.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luval.WebGPT.Controllers
{
    
    public class NotificationController : Controller
    {
        private readonly IAppDbContext _appDbContext;

        public NotificationController(IAppDbContext appDbContext)
        {
                _appDbContext = appDbContext;
        }
        public IActionResult Index()
        {
            var items = _appDbContext.AppUsers.ToList();
            return Json(items);
        }
    }
}
