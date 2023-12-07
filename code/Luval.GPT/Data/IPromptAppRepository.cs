using Luval.GPT.Data.Entities;

namespace Luval.GPT.Data
{
    public interface IPromptAppRepository
    {
        Task<MessageAccount?> GetApplicationUser(string providerName, string providerKey, CancellationToken cancellation);
        Task<IEnumerable<AppMessage>> GetLastConversationHistory(AppMessage message, int? numberOfRecords, CancellationToken cancellation);
        Task<AppMessage> PersistMessageAsync(AppMessage message, CancellationToken cancellation);

        Task<IEnumerable<AppMessage>> GetFirstConversationHistory(AppMessage message, int? top, CancellationToken cancellation);

        IEnumerable<PushAgent> GetPushAgents();
        IEnumerable<PushAgentSubscription> GetSubscriptions(ulong agentId, string userId);
        IEnumerable<PushAgentSubscription> GetSubscriptions(ulong agentId);
    }
}