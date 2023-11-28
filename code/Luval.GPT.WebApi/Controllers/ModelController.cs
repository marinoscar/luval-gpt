using Luval.GPT.Channels;
using Luval.GPT.Channels.Whatsapp;
using Luval.GPT.WebApi.Config;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Luval.GPT.WebApi.Controllers
{
    public class ModelController : Controller
    {

        private readonly ILogger _logger;
        private readonly IMessageClient _messageClient;
        public ModelController(ILogger logger, IMessageClient messageClient)
        {
            _logger = logger;
            _messageClient = messageClient;
        }
        public IActionResult Index()
        {
            _logger.LogDebug("OK");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Connect(IFormCollection formCollection, CancellationToken cancellationToken)
        {
            var h = this.Request.Headers.ToArray();
            var data = WebhookData.FromHttp(formCollection.ToDictionary());
            var payload = JsonConvert.SerializeObject(data);
            var headers = JsonConvert.SerializeObject(h, Formatting.Indented);
            _logger.LogDebug($"\n\nPayload:\n\n {payload}");
            _logger.LogDebug($"\n\nHeaders:\n\n {headers}");

            return await Task.Run(() => { return new OkResult(); }, cancellationToken);
        }
    }
}
