using Luval.Framework.Services;
using Luval.GPT.Channels;
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
    public class ReminderAgentGptService : LuvalServiceBase<ReminderAgentInput, IEnumerable<AppMessage>>
    {
        private readonly IPromptAppRepository _appRepository;
        private readonly PromptAgentService _promptAgentService;
        private readonly MessageService _messageService;

        public ReminderAgentGptService(ILogger logger, string name, IPromptAppRepository repository, PromptAgentService promptService, MessageService messageService, ServiceConfiguration serviceConfiguration) : base(logger, name, serviceConfiguration)
        {
            _appRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _promptAgentService = promptService ?? throw new ArgumentNullException(nameof(promptService));
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        }

        protected override async Task<ServiceResponse<IEnumerable<AppMessage>>> DoExecuteAsync(ReminderAgentInput input, CancellationToken cancellationToken)
        {
            var messages = new List<AppMessage>();
            var response = new ServiceResponse<IEnumerable<AppMessage>>() { Status = ServiceStatus.Completed, Message = string.Empty };

            if (input == null) throw new ArgumentNullException(nameof(input));

            var top = 15;
            if (ServiceConfiguration.Settings.ContainsKey("HistoryCount")) int.TryParse(ServiceConfiguration.Settings["HistoryCount"], out top);

            foreach (var item in input.ProviderKeys)
            {
                var userMessage = new AppMessage() { ChatType = Name, ProviderName = input.Provider, ProviderKey = item, UtcDateTime = DateTime.UtcNow };
                var history = await _appRepository.GetFirstConversationHistory(userMessage, top, cancellationToken);
                if (history == null || !history.Any())
                {
                    history = new List<AppMessage>();
                    userMessage.UserPrompt = input.AgentText;
                }
                else
                {
                    userMessage.UserPrompt = input.ReminderText;
                }
                var result = await _promptAgentService.ExecuteAsync(
                                    new PromptAgentServiceInput()
                                   { Message = userMessage, History = history }
                                   ,cancellationToken);
                if (result.Status == ServiceStatus.Fail)
                {
                    response.Message += "\nUnable to execute prompt for " + item;
                    response.Status = ServiceStatus.Incomplete;
                    response.Exception = response.Exception;
                }
                else if(result.Result != null)
                {
                    messages.Add(result.Result);
                    var msgResponse = await _messageService.ExecuteAsync(new MessageInput() { SenderId = item, Message = result.Result.AgentText }, cancellationToken);
                    if(msgResponse.Status == ServiceStatus.Fail)
                    {
                        response.Message += "\nUnable to send message for " + item;
                        response.Status = ServiceStatus.Incomplete;
                        response.Exception = response.Exception;

                    }
                }
                Task.Delay(500).Wait(); //to avoid rate limits
            }
            if (response.Message == string.Empty) response.Message = "SUCCESS";
            return response;
        }
    }

    public class ReminderAgentInput
    {
        public ReminderAgentInput()
        {
            Provider = ChannelProviders.Whatsapp;
            ProviderKeys = new List<string>();
            ResponseEncloseCharacters = "$";
        }
        public string? AgentText { get; set; }
        public string? ReminderText { get; set; }
        public string Provider { get; set; }
        public string ResponseEncloseCharacters { get; set; }
        public List<string> ProviderKeys { get; set; }
    }
}
