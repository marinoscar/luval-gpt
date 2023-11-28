using Luval.GPT.Channels;
using Luval.GPT.Channels.Whatsapp;
using Luval.GPT.Services;
using Luval.GPT.WebApi.Config;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Luval.GPT.WebApi.Controllers
{
    public class ModelController : Controller
    {

        private readonly ILogger _logger;
        private readonly IMessageClient _messageClient;
        private readonly ChatAgentService _chatAgentService;
        public ModelController(ILogger logger, IMessageClient messageClient, ChatAgentService agentService)
        {
            _logger = logger;
            _messageClient = messageClient;
            _chatAgentService = agentService;
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
            var message = data.ToAppMessage();
            
            var aiReponse = await _chatAgentService.ExecuteAsync(message, cancellationToken);
            var messageResponse = await _messageClient.SendTextMessageAsync(message?.ProviderKey, message?.AgentText, cancellationToken);

            return await Task.Run(() => { return new OkResult(); }, cancellationToken);
        }
    }
}
