using Luval.Framework.Core.Configuration;
using Luval.Framework.Services;
using Luval.Framework.Services.Utilities;
using Luval.GPT.Channels.Push;
using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using WebPush;

namespace Luval.GPT.Services
{
    public class PushAgentChronService : TimedHostedService
    {
        private List<SubscriptionData> _subscriptionData;
        private readonly IPromptAppRepository _repository;
        private readonly uint _subscriptionRefreshInterval;
        private DateTime? _lastUpdate;
        private readonly PushAgentGptManager _pushManager;
        private readonly PushClient _pushClient;

        #region Constructors

        public PushAgentChronService(ILogger logger, IPromptAppRepository repository, PushAgentGptManager pushManager, PushClient pushClient) : this(logger, repository, pushManager, pushClient, TimeSpan.FromMinutes(1))
        {

        }

        public PushAgentChronService(ILogger logger, IPromptAppRepository repository, PushAgentGptManager pushManager, PushClient pushClient, TimeSpan period)
            : this(logger, repository, pushManager, pushClient, DateTime.UtcNow.AddMinutes(1).Subtract(DateTime.UtcNow), period)
        {

        }

        public PushAgentChronService(ILogger logger, IPromptAppRepository repository, PushAgentGptManager pushManager, PushClient pushClient, TimeSpan dueTime, TimeSpan period) : this(logger, repository, pushManager, pushClient, 30, dueTime, period)
        {
        }

        public PushAgentChronService(ILogger logger, IPromptAppRepository repository, PushAgentGptManager pushManager, PushClient pushClient, uint subscriptionRefreshInterval, TimeSpan dueTime, TimeSpan period) : base(logger, dueTime, period)
        {
            _subscriptionRefreshInterval = subscriptionRefreshInterval;
            _pushManager = pushManager ?? throw new ArgumentNullException(nameof(pushManager));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _subscriptionData = new List<SubscriptionData>();
            _pushClient = pushClient ?? throw new ArgumentNullException(nameof(pushClient));
        }


        #endregion
        private void InitSubscription()
        {
            if (_lastUpdate != null && DateTime.UtcNow.Subtract(_lastUpdate.Value).TotalMinutes < _subscriptionRefreshInterval) return;
            var newList = new List<SubscriptionData>();
            var agents = _repository.GetPushAgents();
            foreach (var agent in agents)
            {
                var subs = _repository.GetSubscriptions(agent.Id);
                if (subs != null && subs.Any())
                    foreach (var sub in subs)
                    {
                        newList.Add(new SubscriptionData()
                        {
                            Agent = agent,
                            Subscription = sub,
                            Evaluator = new ChronEvaluator(agent.ChronExpression, agent.Timezone ?? "Central Standard Time")
                        });
                    }
            }
            _subscriptionData = new List<SubscriptionData>(newList);
            _lastUpdate = DateTime.UtcNow;
        }

        protected override void OnTimerTick(object? state)
        {
            InitSubscription(); //Loads all of the subscription data
            foreach (var sub in _subscriptionData)
            {
                if (sub != null &&
                    sub.Subscription != null &&
                    !string.IsNullOrEmpty(sub.Subscription.P256DH) &&
                    !string.IsNullOrEmpty(sub.Subscription.Auth) &&
                    !string.IsNullOrEmpty(sub.Subscription.Endpoint) &&
                    sub.Evaluator != null &&
                    sub.Evaluator.EvaluateNow(false, false))
                {
                    System.Threading.Tasks.Task.Run(() => RunPushAgent(sub));
                }
            }
        }

        private void RunPushAgent(SubscriptionData agentData)
        {
            var managerTask = _pushManager.ProcessPushAgentAsync(agentData.Agent, CancellationToken.None);
            var res = managerTask.Result;
            var options = res.GetOptions(ConfigManager.Get("WebPushUrl"), null);
            _pushClient.Send(options,
                new PushSubscription(agentData.Subscription.Endpoint,
                agentData.Subscription.P256DH,
                agentData.Subscription.Auth));
        }

        protected override void DoWork()
        {

        }
        private class SubscriptionData
        {
            public Data.Entities.PushAgent? Agent { get; set; }
            public PushAgentSubscription? Subscription { get; set; }
            public ChronEvaluator? Evaluator { get; set; }
            DateTime UtcCreatedOn { get; set; }
        }
    }
}
