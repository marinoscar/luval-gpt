using Luval.GPT.Data.Entities;
using Luval.GPT.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Services
{
    public class AgentGptService
    {
        private readonly ChatAgentService _chatAgentService;
        private readonly MessageService _messageService;

        public AgentGptService(ChatAgentService chatAgentService, MessageService messageService)
        {
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _chatAgentService = chatAgentService ?? throw new ArgumentNullException(nameof(chatAgentService));
        }

        public async Task ExecuteAsync(AppMessage message, CancellationToken cancellationToken)
        {
            var aiReponse = await _chatAgentService.ExecuteAsync(message, cancellationToken);
            if (aiReponse != null && aiReponse.Status == Framework.Services.ServiceStatus.Fail) throw new Exception("Unable to execute the chat service");
            var agentText = StringHelper.SplitByWords(message.AgentText, 1500);
            foreach (var text in agentText)
            {
                var messageResponse = await _messageService.ExecuteAsync(new MessageInput() { SenderId = message.ProviderKey, Message = text }, cancellationToken);
            }
        }
    }
}
