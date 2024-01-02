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
        private TimeZoneInfo _timeZoneInfo;
        private readonly IRepository _repository;

        public AgentPresenter(ILogger logger, IRepository repository, IHttpContextAccessor context, ICacheProvider<string, AppUser> userCache, PushAgentGptManager gptManager) : 
            base(logger, repository, context, userCache)
        {
            _gptManager = gptManager ?? throw new ArgumentNullException(nameof(gptManager));
            _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            _repository = repository;
        }

        public IEnumerable<PushAgent> GetAgents()
        {
            var appUser = GetAppUser();
            if (appUser == null) throw new InvalidDataException("User not registered in the app");
            var items = Repository.GetPushAgents(appUser.Id).ToList();
            return items;
        }

        public IEnumerable<string> GetLastMessageIds(string? agentId)
        {
            if(string.IsNullOrEmpty(agentId)) return Array.Empty<string>();

            var items = Repository.GetLastAgentMessageIds(Convert.ToUInt64(agentId), 10);
            return items.Select(x => x.ToString()); 
        }

        public async Task<ServiceResponse<AppMessage>> LoadMessage(string? agentId, string? messageId)
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
            return response;
        }


        public async Task<AppMessage> CreateMessage(PushAgent agent)
        {
            var value = await _gptManager.ProcessPushAgentAsync(agent);
            return value.MessageContent;
        }

        public AppMessage UpVote(ulong messageId)
        {
            return _repository.UpVote(messageId);
        }

        private static AppMessage CleanMessage(AppMessage m)
        {
            var clone = m.Clone();

            clone.AgentText = clone.AgentText.GetTextInBetween("^^^")
                .Replace("[Your Name]", "");

            var o = OptionActionModel.FromGpt(clone.MessageData);
            clone.MessageData = o.Title.Replace("^^^", "");
            return clone;
        }

        public string GetCreatedTimeString(DateTime? utcCreatedOn)
        {
            if (utcCreatedOn == null) return string.Empty;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZoneInfo);
            var createdOnLocal = TimeZoneInfo.ConvertTimeFromUtc(utcCreatedOn.Value.TrimMs(), _timeZoneInfo);
            var span = localTime.Subtract(createdOnLocal);

            if(Convert.ToInt64(span.TotalDays) > 365)
            {
                var years = (span.TotalDays / 365);
                return GetDuration(years, "year");
            }
            if(Convert.ToInt64(span.TotalDays) > 0)
            {
                return GetDuration(span.TotalDays, "day");
            }
            if (Convert.ToInt64(span.TotalHours) > 0)
            {
                return GetDuration(span.TotalHours, "hour");
            }
            if (Convert.ToInt64(span.TotalMinutes) > 0)
            {
                return GetDuration(span.TotalMinutes, "min");
            }
            return GetDuration(span.TotalSeconds, "sec");
        }

        private string GetDuration(double span, string part)
        {
            var val = Convert.ToInt32(span);
            return val == 1 ? $"{val} {part} ago" : $"{val} {part}s ago";
        }
    }
}
