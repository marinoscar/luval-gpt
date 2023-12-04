using Luval.WebPush.Data;
using Luval.WebPush.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Web;
using WebPush;

namespace Luval.WebPush.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.applicationServerKey = _configuration["VAPID:publicKey"];
            return View();
        }

        [HttpPost]
        public IActionResult Index(string client, string endpoint, string p256dh, string auth)
        {
            if (client == null)
            {
                return BadRequest("No Client Name parsed.");
            }
            if (PersistentStorage.GetClientNames().Contains(client))
            {
                return BadRequest("Client Name already used.");
            }
            var subscription = new PushSubscription(endpoint, p256dh, auth);
            PersistentStorage.SaveSubscription(client, subscription);
            return View("Notify", PersistentStorage.GetClientNames());
        }

        public IActionResult Notify()
        {
            return View(PersistentStorage.GetClientNames());
        }

        [HttpPost]
        public IActionResult Notify(string message, string client)
        {
            if (client == null)
            {
                return BadRequest("No Client Name parsed.");
            }
            var subscription = PersistentStorage.GetSubscription(client);
            if (subscription == null)
            {
                return BadRequest("Client was not found");
            }

            var subject = _configuration["VAPID:subject"];
            var publicKey = _configuration["VAPID:publicKey"];
            var privateKey = _configuration["VAPID:privateKey"];

            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);

            var webPushClient = new WebPushClient();
            try
            {
                var payload = GetPayload(message);
                webPushClient.SendNotification(subscription, payload, vapidDetails);
            }
            catch (Exception exception)
            {
                // Log error
                _logger.LogError(exception, "Failed to send notification\n" + exception.Message);
            }

            return View(PersistentStorage.GetClientNames());
        }

        private string GetPayload(string text)
        {
            var payload = new NotificationOptions() { Body = text, Actions = new List<NotificationAction>(), Data = new Dictionary<string, object>() };
            payload.Icon = "~/img/icon1024.png";
            payload.Actions.Add(new NotificationAction() { Action = "explore", Title = "Click to learn more" });
            payload.Actions.Add(new NotificationAction() { Action = "close", Title = "Click to learn more" });
            payload.Data["navigateTo"] = "https://www.google.com/search?q=" + HttpUtility.UrlEncode(text);
            payload.Data["dateOfArrival"] = DateTime.Now.ToString("o");
            payload.Vibrate = new int[] { 100, 50, 100 };

            string json = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return json;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
