using Luval.Framework.Services;
using Luval.GPT.Data.Entities;
using Luval.GPT.GPT;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Services
{
    public class ChatAgentService : LuvalServiceBase<AppMessage, AppMessage>
    {
        private readonly IChatAgent _chatAgent;

        public ChatAgentService(ILogger logger, IChatAgent chatAgent) : this(logger, new ServiceConfiguration() { NumberOfRetries = 3, RetryIntervalInMs = 5000 }, chatAgent)
        {
                
        }

        public ChatAgentService(ILogger logger, ServiceConfiguration serviceConfiguration, IChatAgent chatAgent) : base(logger, "ChatAgentService", serviceConfiguration)
        {
            _chatAgent = chatAgent;
        }

        protected async override Task<ServiceResponse<AppMessage>> DoExecuteAsync(AppMessage input, CancellationToken c)
        {
            var historyCount = 15;
            if (ServiceConfiguration.Settings.ContainsKey("HistoryCount")) int.TryParse(ServiceConfiguration.Settings["HistoryCount"], out historyCount);

            var response = await _chatAgent.ProcessPromptAsync(input, historyCount, c);
            return new ServiceResponse<AppMessage>() { Status = ServiceStatus.Completed, Result = response, Message = "Success" };
        }
    }
}
