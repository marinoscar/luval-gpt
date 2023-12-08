using Luval.GPT.Data.Entities;

namespace Luval.GPT.Data
{
    public interface IRepository
    {
        Task<AppUser?> GetApplicationUser(string providerName, string providerKey, CancellationToken cancellation);
        AppUser? GetApplicationUser(string providerName, string providerKey);


        Task<IEnumerable<AppMessage>> GetLastConversationHistory(AppMessage message, int? numberOfRecords, CancellationToken cancellation);
        Task<AppMessage> PersistMessageAsync(AppMessage message, CancellationToken cancellation);
        Task<Device> RegisterDevice(Device device, CancellationToken cancellation);
        Device RegisterDevice(Device device);

        Task<IEnumerable<AppMessage>> GetFirstConversationHistory(AppMessage message, int? top, CancellationToken cancellation);

        IEnumerable<PushAgent> GetPushAgents();
        IEnumerable<PushAgentSubscription> GetSubscriptions(ulong agentId, string userId);
        IEnumerable<PushAgentSubscription> GetSubscriptions(ulong agentId);
        IEnumerable<Device> GetDevicesFromUser(string userId);
    }
}