using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using Luval.OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;

namespace Luval.GPT.GPT.OpenAI
{
    public class OpenAIChatAgent : IChatAgent
    {

        private readonly IPromptAppRepository _repository;
        private readonly ChatEndpoint _chatEndpoint;

        public OpenAIChatAgent(IPromptAppRepository repository, ChatEndpoint chat)
        {
            _repository = repository;
            _chatEndpoint = chat;
        }

        public async Task<AppMessage> ProcessPromptWithHistoryAsync(AppMessage message, int? historyCount, CancellationToken cancellationToken)
        {
            var history = await _repository.GetLastConversationHistory(message, historyCount, cancellationToken);
            return await ProcessPrompt(message, history, cancellationToken);
        }

        public async Task<AppMessage> ProcessPrompt(AppMessage newMessage, IEnumerable<AppMessage> history, CancellationToken cancellationToken)
        {
            _chatEndpoint.ClearMessages();
            foreach (var chat in history.Where(i => i != null))
            {
                _chatEndpoint.AddUserMessage(chat.UserPrompt);
                _chatEndpoint.AddAssitantMessage(chat.AgentText);
            }
            _chatEndpoint.AddUserMessage(newMessage.UserPrompt);
            var response = await _chatEndpoint.SendAsync();
            newMessage.TokenCount = (uint?)response.Usage.TotalTokens;
            newMessage.AgentText = response.Choice.Message.Content;

            var result = await _repository.PersistMessageAsync(newMessage, cancellationToken);
            return result;
        }
    }
}
