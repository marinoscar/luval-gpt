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
using WebPush;


namespace Luval.GPT.Services
{
    public class PushAgentChronService : TimedHostedService
    {
        private List<SubscriptionData> _subscriptionData;
        private readonly IRepository _repository;
        private readonly uint _subscriptionRefreshInterval;
        private DateTime? _lastUpdate;
        private readonly PushAgentGptManager _pushManager;
        private readonly PushClient _pushClient;

        #region Constructors

        public PushAgentChronService(ILogger logger, IRepository repository, PushAgentGptManager pushManager, PushClient pushClient) : this(logger, repository, pushManager, pushClient, TimeSpan.FromMinutes(1))
        {

        }

        public PushAgentChronService(ILogger logger, IRepository repository, PushAgentGptManager pushManager, PushClient pushClient, TimeSpan period)
            : this(logger, repository, pushManager, pushClient, DateTime.UtcNow.AddMinutes(1).Subtract(DateTime.UtcNow), period)
        {

        }

        public PushAgentChronService(ILogger logger, IRepository repository, PushAgentGptManager pushManager, PushClient pushClient, TimeSpan dueTime, TimeSpan period) : this(logger, repository, pushManager, pushClient, 30, dueTime, period)
        {
        }

        public PushAgentChronService(ILogger logger, IRepository repository, PushAgentGptManager pushManager, PushClient pushClient, uint subscriptionRefreshInterval, TimeSpan dueTime, TimeSpan period) : base(logger, dueTime, period)
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
                        var devices = _repository.GetDevicesFromUser(sub.AppUserId);
                        newList.Add(new SubscriptionData()
                        {
                            Agent = agent,
                            Subscription = sub,
                            Evaluator = new ChronEvaluator(agent.ChronExpression, agent.Timezone ?? "Central Standard Time"),
                            Devices = devices.ToList(),
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
                    sub.Evaluator != null &&
                    sub.Evaluator.EvaluateNow(false, false))
                {
                    foreach (var d in sub.Devices)
                    {
                        if (d == null) continue;
                        Task.Run(() => RunPushAgent(sub, d));
                    }
                }
            }
        }

        private void RunPushAgent(SubscriptionData agentData, Device device)
        {
            var managerTask = _pushManager.ProcessPushAgentAsync(agentData.Agent);
            var res = managerTask.Result;
            var options = res.GetOptions(ConfigManager.Get("WebPushUrl"), null);
            _pushClient.Send(options,
                new PushSubscription(device.Endpoint,
                device.P256DH,
                device.Auth));

            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
        }

        protected override void DoWork()
        {

        }
        private class SubscriptionData
        {
            public SubscriptionData()
            {
                Devices = new List<Device>();
            }
            public Data.Entities.PushAgent? Agent { get; set; }
            public PushAgentSubscription? Subscription { get; set; }
            public ChronEvaluator? Evaluator { get; set; }

            public List<Device> Devices { get; set; }
            DateTime UtcCreatedOn { get; set; }
        }
    }
}
