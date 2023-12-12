using Luval.GPT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Data
{
    public class AppRepository : IRepository
    {

        private readonly IAppDbContext _dbContext;

        public AppRepository(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AppMessage> PersistMessageAsync(AppMessage message)
        {
            await _dbContext.AppMessages.AddAsync(message);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            return message;
        }

        public async Task<int> UpdateOrCreatePushAgent(IEnumerable<PushAgent> agents)
        {
            foreach (var agent in agents)
            {
                if(agent.Id <= 0)
                    await CreateAgent(agent);
                else
                    await UpdateAgent(agent);
            }
            return agents.Count();
        }

        public async Task<PushAgent> CreateAgent(PushAgent agent)
        {
            await _dbContext.PushAgents.AddAsync(agent);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            await _dbContext.PushAgentSubscriptions.AddAsync(new PushAgentSubscription()
            {
                AppUserId = agent.AppUserId, PushAgentId = agent.Id
            });
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            return agent;
        }

        public async Task<PushAgent> UpdateAgent(PushAgent agent)
        {
            var entity = _dbContext.PushAgents.Attach(agent);
            entity.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            return agent;
        }

        public Device RegisterDevice(Device device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));

            var item = _dbContext.Devices.FirstOrDefault(
                i => i.P256DH == device.P256DH &&
                i.Endpoint == device.Endpoint &&
                i.Auth == device.Auth);
            if (item != null)
                return item;

            _dbContext.Devices.Add(device);

            _dbContext.SaveChanges();

            return device;
        }

        public AppUserPurpose? GetPurpose(string userId)
        {
            return _dbContext.AppUserPurposes.FirstOrDefault(i => i.AppUserId == userId);
        }

        public void UpdateAppMessage(AppMessage message)
        {
            var i = _dbContext.AppMessages.Attach(message);
            i.State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public AppUser? GetApplicationUser(string providerName, string providerKey)
        {
            return _dbContext.AppUsers.FirstOrDefault(i => i.ProviderName == providerName && i.ProviderKey == providerKey);
        }

        public async Task<IEnumerable<AppMessage>> GetLastConversationHistory(AppMessage message, int? lastNumberOfRecords)
        {
            var predicate = GetPredicate(message);

            if (lastNumberOfRecords == null) return await GetAllMessagesAsync(predicate);

            var recordCount = await GetNumberofChatRecordsAsync(predicate);

            var delta = recordCount - lastNumberOfRecords;
            if (delta < recordCount) return await GetAllMessagesAsync(predicate);

            return await Task.Run(() =>
            {
                return _dbContext.AppMessages.Where(predicate).Skip(delta.Value);
            });
        }

        public async Task<IEnumerable<AppMessage>> GetFirstConversationHistory(AppMessage message, int? top)
        {
            var predicate = GetPredicate(message);

            if (top == null) return await GetAllMessagesAsync(predicate);

            return await Task.Run(() =>
            {
                return _dbContext.AppMessages.Where(predicate).Take((int)top);
            });
        }

        public IEnumerable<PushAgent> GetPushAgents()
        {
            return _dbContext.PushAgents.Where(i => i.Id > 0).ToList();
        }

        public IEnumerable<PushAgent> GetPushAgents(string userId)
        {
            return _dbContext.PushAgents.Where(i => i.AppUserId == userId);
        }


        public AppMessage GetAppMessage(ulong id)
        {
            return _dbContext.AppMessages.SingleOrDefault(i => i.Id == id);
        }

        public PushAgent GetPushAgent(ulong id)
        {
            return _dbContext.PushAgents.SingleOrDefault(i => i.Id == id);
        }

        public AppMessage GetAgentMessage(ulong messageId)
        {
            return _dbContext.AppMessages.FirstOrDefault(i => i.Id == messageId);
        }

        public IEnumerable<PushAgentSubscription> GetSubscriptions(ulong agentId, string userId)
        {
            return _dbContext.PushAgentSubscriptions.Where(i => i.PushAgentId == agentId && i.AppUserId == userId);
        }

        public IEnumerable<Device> GetDevicesFromUser(string userId)
        {
            return _dbContext.Devices.Where(i => i.AppUserId == userId);
        }

        public IEnumerable<PushAgentSubscription> GetSubscriptions(ulong agentId)
        {
            return _dbContext.PushAgentSubscriptions.Where(i => i.PushAgentId == agentId);
        }

        #region Private Methods

        private Func<AppMessage, bool> GetPredicate(AppMessage message)
        {
            Func<AppMessage, bool> predicate = i => i.ProviderKey == message.ProviderKey && i.ProviderName == message.ProviderName && i.ChatType == message.ChatType;
            return predicate;
        }

        private Task<int> GetNumberofChatRecordsAsync(Func<AppMessage, bool> predicate)
        {
            return Task.Run(() =>
            {
                return _dbContext.AppMessages.Count(predicate);
            });
        }

        private Task<IEnumerable<AppMessage>> GetAllMessagesAsync(Func<AppMessage, bool> predicate)
        {
            return Task.Run(() => { return _dbContext.AppMessages.Where(predicate); });
        }

        #endregion


    }
}
