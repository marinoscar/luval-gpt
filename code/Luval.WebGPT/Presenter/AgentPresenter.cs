using Luval.Framework.Core;
using Luval.Framework.Services;
using Luval.GPT.Channels.Push.Models;
using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using Luval.GPT.Services;

namespace Luval.WebGPT.Presenter
{
    public class AgentPresenter
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;
        private readonly HttpContext _context;
        private readonly IHttpContextAccessor _contextA;
        private readonly PushAgentGptManager _gptManager;
        private AppUser? _appUser;

        public AgentPresenter(ILogger logger, IRepository repository, IHttpContextAccessor context, PushAgentGptManager gptManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _gptManager = gptManager ?? throw new ArgumentNullException(nameof(gptManager));
            _contextA = context ?? throw new ArgumentNullException(nameof(context));

            _context = _contextA?.HttpContext;
        }

        public IEnumerable<PushAgent> GetAgents()
        {
            var appUser = GetAppUser();
            var items = _repository.GetPushAgents(appUser.Id).ToList();
            return items;
        }

        public async Task<ServiceResponse<AppMessage>> LoadMessage(string? agentId, string? messageId, Action<ServiceResponse<AppMessage>> callback)
        {
            var response = new ServiceResponse<AppMessage>()
            {
                Status = ServiceStatus.Completed
            };
            if (agentId == null) throw new ArgumentNullException(nameof(agentId));
            try
            {
                if (messageId == null)
                {
                    var agent = _repository.GetPushAgent(Convert.ToUInt64(agentId));
                    response.Result = CleanMessage(await CreateMessage(agent));
                    response.Message = "Message created";
                }
                else
                {
                    response.Result = CleanMessage(_repository.GetAppMessage(Convert.ToUInt64(messageId)));
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

        private AppUser GetAppUser()
        {
            if (_appUser != null) return _appUser;
            var webUser = _context.User.ToUser();
            _appUser = _repository.GetApplicationUser(webUser.ProviderName, webUser.ProviderKey);
            return _appUser;
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
