using Luval.Framework.Services;
using Luval.GPT.Channels;
using Luval.GPT.GPT;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Services
{
    public class MessageService: LuvalServiceBase<MessageInput, ChannelMessageResponse>
    {

        private readonly IMessageClient _messageClient;

        public MessageService(ILogger logger, IMessageClient messageClient) : this(logger, new ServiceConfiguration() { NumberOfRetries = 3, RetryIntervalInMs = 5000 }, messageClient)
        {

        }

        public MessageService(ILogger logger, ServiceConfiguration serviceConfiguration, IMessageClient messageClient) : base(logger, "MessageService", serviceConfiguration)
        {
            _messageClient = messageClient;
        }

        protected override async Task<ServiceResponse<ChannelMessageResponse>> DoExecuteAsync(MessageInput input, CancellationToken cancellationToken)
        {
            var response = await _messageClient.SendTextMessageAsync(input.SenderId, input.Message, cancellationToken);
            return new ServiceResponse<ChannelMessageResponse>() { Result = response, Status = ServiceStatus.Completed, Message = "Success" };
        }
    }

    public class MessageInput
    {
        public string SenderId { get; set; }
        public string Message { get; set; }
    }
}
