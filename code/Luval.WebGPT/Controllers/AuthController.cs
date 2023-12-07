using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace Luval.WebGPT.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            
            //if (string.IsNullOrEmpty(redirectUrl)) redirectUrl = "/";
            var prop = new AuthenticationProperties()
            {
                RedirectUri = "/"
            };
            return Challenge(prop, GoogleDefaults.AuthenticationScheme);
        }
    }
}
