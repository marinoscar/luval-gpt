using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using Luval.OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.GPT.OpenAI
{
    public class OpenAIChatAgent : IChatAgent
    {

        private readonly IAppRepository _repository;
        private readonly ChatEndpoint _chatEndpoint;

        public OpenAIChatAgent(IAppRepository repository, ChatEndpoint chat)
        {
            _repository = repository;
            _chatEndpoint = chat;
        }

        public async Task<AppMessage> ProcessPromptAsync(AppMessage message, int? historyCount, CancellationToken cancellationToken)
        {
            var history = await _repository.GetConversationHistory(message, historyCount, cancellationToken);
            _chatEndpoint.ClearMessages();
            foreach (var chat in history.Where(i => i != null))
            {
                _chatEndpoint.AddUserMessage(chat.UserPrompt);
                _chatEndpoint.AddAssitantMessage(chat.AgentText);
            }
            _chatEndpoint.AddUserMessage(message.UserPrompt);
            var response = await _chatEndpoint.SendAsync();
            message.TokenCount = (uint?)response.Usage.TotalTokens;
            message.AgentText = response.Choice.Message.Content;

            var result = await _repository.PersistMessageAsync(message, cancellationToken);
            return result;
        }
    }
}
