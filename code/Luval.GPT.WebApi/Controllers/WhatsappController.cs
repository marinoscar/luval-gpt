using Microsoft.AspNetCore.Mvc;
using Luval.GPT.Channels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Luval.GPT.WebApi.Controllers
{
    public class WhatsappController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMessageClient _messageClient;
        public WhatsappController(ILogger logger, IMessageClient messageClient)
        {
            _logger = logger;
            _messageClient = messageClient;
        }

        [HttpPost("Recieve")]
        public IActionResult Recieve(FormCollection formCollection)
        {
            var content = JsonConvert.SerializeObject(formCollection);
            _logger.LogDebug($"Payload:\n\n {content}");

            return new OkResult();
        }

        [HttpGet("Send")]
        public IActionResult Send()
        {
            return new JsonResult("Good");
        }
    }
}
