using Luval.Framework.Core.Configuration;
using Luval.Framework.Services;
using Luval.GPT.Channels.Push.Models;
using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Services
{
    public class PushAgentGptManager
    {
        private readonly IRepository _repository;
        private readonly PromptAgentService _agentService;
        public PushAgentGptManager(IRepository repository, PromptAgentService promptAgent)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _agentService = promptAgent ?? throw new ArgumentNullException(nameof(promptAgent));
        }

        public async Task<WebPushResponse> ProcessPushAgentAsync(PushAgent agent)
        {
            var purpose = _repository.GetPurpose(agent.AppUserId);
            string purposeText;
            
            if (purpose == null)
                purposeText = GetGenericPurpose();
            else
                purposeText = purpose.Purpose;

            var gptMessage = await GetMessageAsync(agent, purposeText);
            var gptNotification = await GetMessageForNotification(gptMessage);

            gptMessage.MessageData = gptNotification.AgentText;
            _repository.UpdateAppMessage(gptMessage);

            return new WebPushResponse() { MessageContent = gptMessage, NotificationContent = gptNotification };
        }

        public async Task<AppMessage> GetMessageAsync(PushAgent agent, string purpose)
        {
            var history = (await _repository.GetFirstConversationHistory(CreateRootMessage(agent, purpose), 20)).ToList();

            var message = history.Any() ? CreateFollowUpMessage(agent, purpose) : CreateRootMessage(agent, purpose);
            var response = await _agentService.ExecuteAsync(new PromptAgentServiceInput() { History = history, Message = message }, CancellationToken.None);
            if (response.Status != ServiceStatus.Completed) throw new Exception($"Unable to complete prompt request. Error: {response.Message}", response.Exception);
            return response.Result;
        }

        private async Task<AppMessage> GetMessageForNotification(AppMessage appMessage)
        {
            var promptMessage = RequestNotificationOptions(appMessage.AgentText, appMessage.ProviderKey);
            var response = await _agentService.ExecuteAsync(new PromptAgentServiceInput() { Message = promptMessage, History = new List<AppMessage>() }, CancellationToken.None);
            if (response.Status != ServiceStatus.Completed) throw new Exception($"Unable to complete prompt request. Error: {response.Message}", response.Exception);
            return response.Result;
        }

        private AppMessage RequestNotificationOptions(string agentText, string userId)
        {
            var text = @$"
This message will be part of an application to provide motivation to the users, I need to create a notification for the mobile apps that will new following information

Title: Short text to allow the user to click
Call to Action: very short text to motivate the user to click on the notification and read the content, no more than 5 words, include an emoji at the beginning
Decline button text: Funny and short text to ignore the message, include an emoji at the beginning

Here is the message:

""{agentText}""

provide the information in json format with the following structure
key: title
key: callToAction
key: decline
";
            return new AppMessage()
            {
                UserPrompt = text,
                ChatType = "PushNotificationOptions",
                ProviderName = "PushNotificationOptions",
                ProviderKey = userId,
                UtcDateTime = DateTime.UtcNow,
            };
        }


        private AppMessage CreateRootMessage(PushAgent agent, string purpose)
        {
            return CreateMessage(agent, agent.RootMessage, purpose);
        }

        private AppMessage CreateFollowUpMessage(PushAgent agent, string purpose)
        {
            return CreateMessage(agent, agent.UserPrompt, purpose);
        }

        private AppMessage CreateMessage(PushAgent agent, string rootText, string purpose)
        {
            var text = @$"
{agent.PromptPrefix}

{rootText}

Make sure the message is aligned and use my personal purpose that is as follows:
{purpose}

{agent.PromptSuffix}

";

            return new AppMessage()
            {
                UserPrompt = text,
                ChatType = "PushNotification",
                ProviderName = agent.GetProviderName(),
                ProviderKey = agent.AppUserId,
                UtcDateTime = DateTime.UtcNow,
            };
        }

        private string GetGenericPurpose()
        {
            return ConfigManager.GetOrDefault("GenericPurpose", "Be a better person and be more healthy");
        }

    }
}
