
using Luval.GPT.Channels.Push.Models;
using Luval.GPT.Data.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;
using WebPush;

namespace Luval.GPT.Channels.Push
{
    public class PushClient
    {
        private readonly VapidDetails _vapi;
        private readonly ILogger _logger;
        public PushClient(ILogger logger, VapidDetails vapi)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _vapi = vapi ?? throw new ArgumentNullException(nameof(vapi));
        }

        public Task SendAsync(NotificationOptions options, PushSubscription subscription, CancellationToken cancellationToken)
        {
            return Task.Run(() => { Send(options, subscription); }, cancellationToken);
        }

        public void Send(NotificationOptions options,  PushSubscription subscription)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (subscription == null) throw new Exception(nameof(subscription));

            var webPushClient = new WebPushClient();
            try
            {
                var payload = options.ToString();
                webPushClient.SendNotification(subscription, payload, _vapi);
            }
            catch (Exception exception)
            {
                // Log error
                _logger.LogError(exception, "Failed to send notification\n" + exception.Message);
            }
        }
    }
}
