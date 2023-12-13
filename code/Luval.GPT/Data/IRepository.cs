using Luval.GPT.Data.Entities;

namespace Luval.GPT.Data
{
    public interface IRepository
    {


        Task<int> UpdateOrCreatePushAgent(IEnumerable<PushAgent> agents);
        Task<PushAgent> CreateAgent(PushAgent agent);
        Task<PushAgent> UpdateAgent(PushAgent agent);
        AppUser? GetApplicationUser(string providerName, string providerKey);
        AppUser? GetApplicationUser(string userId);


        Task<IEnumerable<AppMessage>> GetLastConversationHistory(AppMessage message, int? numberOfRecords);
        Task<AppMessage> PersistMessageAsync(AppMessage message);
        Device RegisterDevice(Device device);
        void UpdateAppMessage(AppMessage message);

        Task<IEnumerable<AppMessage>> GetFirstConversationHistory(AppMessage message, int? top);


        AppUserPurpose? GetPurpose(string userId);
        AppMessage GetAppMessage(ulong id);
        IEnumerable<PushAgent> GetPushAgents();
        IEnumerable<PushAgent> GetPushAgents(string userId);
        PushAgent GetPushAgent(ulong id);

        IEnumerable<PushAgentSubscription> GetSubscriptions(ulong agentId, string userId);
        IEnumerable<PushAgentSubscription> GetSubscriptions(ulong agentId);
        IEnumerable<Device> GetDevicesFromUser(string userId);
    }
}