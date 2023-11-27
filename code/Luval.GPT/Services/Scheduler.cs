using Luval.Framework.Services;
using Luval.GPT.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Services
{
    public class Scheduler : TimedHostedService
    {

        private readonly IMessageClient _messageClient;

        public Scheduler(ILogger logger, IMessageClient messageClient) : base(logger, TimeSpan.FromMinutes(1))
        {
            _messageClient = messageClient;
        }

        protected async override void DoWork()
        {
            Logger.LogDebug($"Sending message to: \"+12488057580\"");
            var item = await _messageClient.SendTextMessageAsync("+12488057580", $"Ping: {ExecutionCount}", CancellationToken.None);
            Logger.LogDebug($"Sent with response:\n\n{item.ToString()}");
        }
    }
}
