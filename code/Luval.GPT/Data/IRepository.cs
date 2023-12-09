using Luval.GPT.Data.Entities;

namespace Luval.GPT.Data
{
    public interface IRepository
    {
        
        AppUser? GetApplicationUser(string providerName, string providerKey);


        Task<IEnumerable<AppMessage>> GetLastConversationHistory(AppMessage message, int? numberOfRecords);
        Task<AppMessage> PersistMessageAsync(AppMessage message);
        Device RegisterDevice(Device device);
        void UpdateAppMessage(AppMessage message);

        Task<IEnumerable<AppMessage>> GetFirstConversationHistory(AppMessage message, int? top);

        AppMessage GetAppMessage(ulong id);
        IEnumerable<PushAgent> GetPushAgents();
        IEnumerable<PushAgent> GetPushAgents(string userId);
        PushAgent GetPushAgent(ulong id);

        IEnumerable<PushAgentSubscription> GetSubscriptions(ulong agentId, string userId);
        IEnumerable<PushAgentSubscription> GetSubscriptions(ulong agentId);
        IEnumerable<Device> GetDevicesFromUser(string userId);
    }
}