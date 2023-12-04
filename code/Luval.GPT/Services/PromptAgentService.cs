using Luval.Framework.Services;
using Luval.GPT.Data.Entities;
using Luval.GPT.GPT;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Services
{
    public class PromptAgentService : LuvalServiceBase<PromptAgentServiceInput, AppMessage>
    {
        private readonly IChatAgent _chatAgent;

        public PromptAgentService(ILogger logger, IChatAgent chat) : base(logger, nameof(PromptAgentService), new ServiceConfiguration() { NumberOfRetries = 3, RetryIntervalInMs = 3000 })
        {
            _chatAgent = chat;
        }

        public PromptAgentService(ILogger logger, IChatAgent chat, ServiceConfiguration serviceConfiguration) : base(logger, nameof(PromptAgentService), serviceConfiguration)
        {
            _chatAgent = chat;
        }

        protected override async Task<ServiceResponse<AppMessage>> DoExecuteAsync(PromptAgentServiceInput input, CancellationToken cancellationToken)
        {
            var response = await _chatAgent.ProcessPrompt(input.Message, input.History, cancellationToken);

            return new ServiceResponse<AppMessage>() { Status = ServiceStatus.Completed, Result = response, Message = "Success" };
        }
    }

    public class PromptAgentServiceInput
    {
        public PromptAgentServiceInput() : this(default(AppMessage), default(IEnumerable<AppMessage>))
        {

        }

        public PromptAgentServiceInput(AppMessage message, IEnumerable<AppMessage> history)
        {
            Message = message;
            History = history;
        }
        public AppMessage? Message { get; set; }
        public IEnumerable<AppMessage>? History { get; set; }
    }
}
