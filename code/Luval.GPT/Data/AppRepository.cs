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

        public async Task<AppMessage> PersistMessageAsync(AppMessage message, CancellationToken cancellation)
        {
            await _dbContext.AppMessages.AddAsync(message, cancellation);
            await _dbContext.SaveChangesAsync(cancellation);
            return message;
        }

        public async Task<Device> RegisterDevice(Device device, CancellationToken cancellation)
        {
            if(device == null) throw new ArgumentNullException(nameof(device));

            var item = await _dbContext.Devices.FirstOrDefaultAsync(
                i => i.P256DH == device.P256DH &&
                i.Endpoint == device.Endpoint &&
                i.Auth == device.Auth, cancellation);
            if (item != null)
                return item;

            await _dbContext.Devices.AddAsync(device, cancellation);

            await _dbContext.SaveChangesAsync(cancellation);

            return device;
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

        public async Task<AppUser?> GetApplicationUser(string providerName, string providerKey, CancellationToken cancellation)
        {
            return await _dbContext.AppUsers.FirstOrDefaultAsync(i => i.ProviderName == providerName && i.ProviderKey == providerKey, cancellation);
        }

        public AppUser? GetApplicationUser(string providerName, string providerKey)
        {
            return _dbContext.AppUsers.FirstOrDefault(i => i.ProviderName == providerName && i.ProviderKey == providerKey);
        }

        public async Task<IEnumerable<AppMessage>> GetLastConversationHistory(AppMessage message, int? lastNumberOfRecords, CancellationToken cancellation)
        {
            var predicate = GetPredicate(message);

            if (lastNumberOfRecords == null) return await GetAllMessagesAsync(predicate, cancellation);

            var recordCount = await GetNumberofChatRecordsAsync(predicate, cancellation);

            var delta = recordCount - lastNumberOfRecords;
            if (delta < recordCount) return await GetAllMessagesAsync(predicate, cancellation);

            return await Task.Run(() =>
            {
                return _dbContext.AppMessages.Where(predicate).Skip(delta.Value);
            }, cancellation);
        }

        public async Task<IEnumerable<AppMessage>> GetFirstConversationHistory(AppMessage message, int? top, CancellationToken cancellation)
        {
            var predicate = GetPredicate(message);

            if (top == null) return await GetAllMessagesAsync(predicate, cancellation);

            return await Task.Run(() =>
            {
                return _dbContext.AppMessages.Where(predicate).Take((int)top);
            }, cancellation);
        }

        public IEnumerable<PushAgent> GetPushAgents()
        {
            return _dbContext.PushAgents;
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



        private Func<AppMessage, bool> GetPredicate(AppMessage message)
        {
            Func<AppMessage, bool> predicate = i => i.ProviderKey == message.ProviderKey && i.ProviderName == message.ProviderName && i.ChatType == message.ChatType;
            return predicate;
        }

        private Task<int> GetNumberofChatRecordsAsync(Func<AppMessage, bool> predicate, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return _dbContext.AppMessages.Count(predicate);
            }, cancellation);
        }

        private Task<IEnumerable<AppMessage>> GetAllMessagesAsync(Func<AppMessage, bool> predicate, CancellationToken cancellation)
        {
            return Task.Run(() => { return _dbContext.AppMessages.Where(predicate); }, cancellation);
        }


    }
}
