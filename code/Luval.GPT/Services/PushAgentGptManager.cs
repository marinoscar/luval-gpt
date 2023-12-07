using Amazon.Runtime.Internal.Util;
using Luval.Framework.Services;
using Luval.GPT.Channels.Push.Models;
using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Services
{
    public class PushAgentGptManager
    {
        private readonly IPromptAppRepository _repository;
        private readonly PromptAgentService _agentService;
        public PushAgentGptManager(ILogger logger, IPromptAppRepository repository, PromptAgentService promptAgent)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _agentService = promptAgent ?? throw new ArgumentNullException(nameof(promptAgent));
        }

        public async Task<WebPushResponse> ProcessPushAgentAsync(PushAgent agent, CancellationToken cancellationToken)
        {
            var gptMessage = await GetMessageAsync(agent, cancellationToken);
            var gptNotification = await GetMessageForNotification(gptMessage, cancellationToken);
            return new WebPushResponse() { MessageContent = gptMessage, NotificationContent = gptNotification };
        }

        public async Task<AppMessage> GetMessageAsync(PushAgent agent, CancellationToken cancellationToken)
        {
            var history = await _repository.GetFirstConversationHistory(CreateRootMessage(agent), 20, cancellationToken);
            var message = history.Any() ? CreateFollowUpMessage(agent) : CreateRootMessage(agent);
            var response = await _agentService.ExecuteAsync(new PromptAgentServiceInput() { History = history, Message = message }, cancellationToken);
            if (response.Status != ServiceStatus.Completed) throw new Exception($"Unable to complete prompt request. Error: { response.Message }", response.Exception);
            return response.Result;
        }

        private async Task<AppMessage> GetMessageForNotification(AppMessage appMessage, CancellationToken cancellationToken)
        {
            var promptMessage = RequestNotificationOptions(appMessage.AgentText, appMessage.ProviderKey);
            var response = await _agentService.ExecuteAsync(new PromptAgentServiceInput() { Message = promptMessage }, cancellationToken);
            if (response.Status != ServiceStatus.Completed) throw new Exception($"Unable to complete prompt request. Error: {response.Message}", response.Exception);
            return response.Result;
        }

        private AppMessage RequestNotificationOptions(string agentText, string userId)
        {
            var text = @$"
This message will be part of an application to provide motivation to the users, I need to create a notification for the mobile apps that will new following information

Title: Short text to allow the user to click
Call to Action: very short text to motivate the user to click on the notification and read the content, include an emoji at the beginning
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


        private AppMessage CreateRootMessage(PushAgent agent)
        {
            return CreateMessage(agent, agent.AgentPurpose);
        }

        private AppMessage CreateFollowUpMessage(PushAgent agent)
        {
            return CreateMessage(agent, agent.UserPrompt);
        }

        private AppMessage CreateMessage(PushAgent agent, string rootText)
        {
            var text = @$"
{agent.PromptPrefix}

{rootText}

{agent.PromptSuffix}

Provide the response between $

";

            return new AppMessage()
            {
                UserPrompt = text,
                ChatType = "PushNotification",
                ProviderName = "PushNotification",
                ProviderKey = agent.AppUserId,
                UtcDateTime = DateTime.UtcNow,
            };
        }

    }
}
