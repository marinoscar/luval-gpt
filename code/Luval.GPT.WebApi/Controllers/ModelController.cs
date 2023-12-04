using Luval.GPT.Channels;
using Luval.GPT.Channels.Whatsapp;
using Luval.GPT.Data.Entities;
using Luval.GPT.Services;
using Luval.GPT.WebApi.Config;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading;
using Luval.GPT.WebApi.Filters;
using Luval.GPT.Data;

namespace Luval.GPT.WebApi.Controllers
{


    public class ModelController : Controller
    {

        private readonly ILogger _logger;
        private readonly QueryAgentGptService _agentService;
        private readonly FireAndForgetHandler _fireAndForget;
        private readonly IAppRepository _appRepository;

        public ModelController(ILogger logger, QueryAgentGptService agentService, FireAndForgetHandler fireAndForget, IAppRepository appRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            _fireAndForget = fireAndForget ?? throw new ArgumentException(nameof(fireAndForget));
            _appRepository = appRepository ?? throw new ArgumentNullException(nameof(appRepository));
        }
        public IActionResult Index()
        {
            _logger.LogDebug("OK");
            return View();
        }

        [HttpPost, ValidateTwilioRequest]
        public async Task<IActionResult> Connect(IFormCollection formCollection, CancellationToken cancellationToken)
        {
            var data = WebhookData.FromHttp(formCollection.ToDictionary());
            var message = data.ToAppMessage();
            var user = await _appRepository.GetApplicationUser(ChannelProviders.Whatsapp, message.ProviderKey, cancellationToken);

            if (user == null)
                return Forbid(); 


            DoProcessMessage(message, cancellationToken);
            _logger.LogInformation($"Queued Request for Message: {data.MessageSid}");
            return Ok();
        }

        public IActionResult Test()
        {
            var message = new AppMessage()
            {
                ProviderKey = "+12488057580",
                ProviderName = "Whatsapp",
                UserPrompt = "write me an essay of about 5000 characters long of the french revolution",
                ChatType = "Test",
                UtcDateTime = DateTime.UtcNow,
                UserMediaType = "0"
            };
            DoProcessMessage(message, CancellationToken.None);

            return Json("Completed");
        }

        private void DoProcessMessage(AppMessage message, CancellationToken cancellationToken)
        {
            _fireAndForget.Execute<QueryAgentGptService>(async (s) =>
            {
                await s.ExecuteAsync(message, cancellationToken);
            });
        }
    }
}
