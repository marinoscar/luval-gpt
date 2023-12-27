using Luval.Framework.Core;
using Luval.Framework.Core.Cache;
using Luval.Framework.Services;
using Luval.GPT.Channels.Push.Models;
using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using Luval.GPT.Services;

namespace Luval.WebGPT.Presenter
{
    public class AgentPresenter : PresenterBase
    {
        private readonly PushAgentGptManager _gptManager;

        public AgentPresenter(ILogger logger, IRepository repository, IHttpContextAccessor context, ICacheProvider<string, AppUser> userCache, PushAgentGptManager gptManager) : 
            base(logger, repository, context, userCache)
        {
            _gptManager = gptManager ?? throw new ArgumentNullException(nameof(gptManager));
        }

        public IEnumerable<PushAgent> GetAgents()
        {
            var appUser = GetAppUser();
            if (appUser == null) throw new InvalidDataException("User not registered in the app");
            var items = Repository.GetPushAgents(appUser.Id).ToList();
            return items;
        }

        public async Task<ServiceResponse<AppMessage>> LoadMessage(string? agentId, string? messageId, Action<ServiceResponse<AppMessage>> callback)
        {
            var response = new ServiceResponse<AppMessage>()
            {
                Status = ServiceStatus.Completed
            };
            try
            {
                if (messageId == null)
                {
                    var agent = Repository.GetPushAgent(Convert.ToUInt64(agentId));
                    response.Result = CleanMessage(await CreateMessage(agent));
                    response.Message = "Message created";
                }
                else
                {
                    response.Result = CleanMessage(Repository.GetAppMessage(Convert.ToUInt64(messageId)));
                    response.Message = "Message loaded";
                }
            }
            catch (Exception e)
            {

                response.Exception = e;
                response.Status = ServiceStatus.Fail;
                response.Message = e.Message;
            }
            callback(response);
            return response;
        }

        public async Task<AppMessage> CreateMessage(PushAgent agent)
        {
            var value = await _gptManager.ProcessPushAgentAsync(agent);
            return value.MessageContent;
        }

        private static AppMessage CleanMessage(AppMessage m)
        {
            var clone = m.Clone();

            clone.AgentText = clone.AgentText.GetTextInBetween("^^^");
            var o = OptionActionModel.FromGpt(clone.MessageData);
            clone.MessageData = o.Title.Replace("^^^", "");
            return clone;
        }
    }
}
