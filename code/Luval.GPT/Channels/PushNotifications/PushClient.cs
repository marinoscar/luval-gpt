using Luval.GPT.Channels.PushNotifications.Models;
using Luval.GPT.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;
using WebPush;

namespace Luval.GPT.Channels.PushNotifications
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

        public Task SendAsync(PushAgentMessage agentMessage, AppUserDevice userDevice, CancellationToken cancellationToken)
        {
            return Task.Run(() => { Send(agentMessage, userDevice); }, cancellationToken);
        }

        public void Send(PushAgentMessage agentMessage, AppUserDevice userDevice)
        {
            if (agentMessage == null) throw new ArgumentNullException(nameof(agentMessage));
            if (userDevice == null) throw new Exception(nameof(userDevice));

            var webPushClient = new WebPushClient();
            try
            {
                var payload = agentMessage.ToNotificationOptions().ToString();
                webPushClient.SendNotification(userDevice.ToPushSub(), payload, _vapi);
            }
            catch (Exception exception)
            {
                // Log error
                _logger.LogError(exception, "Failed to send notification\n" + exception.Message);
            }
        }
    }
}
