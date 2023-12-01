using Luval.GPT.Data.Entities;

namespace Luval.GPT.Data
{
    public interface IAppRepository
    {
        Task<ApplicationUser?> GetApplicationUser(string providerName, string providerKey, CancellationToken cancellation);
        Task<IEnumerable<AppMessage>> GetConversationHistory(AppMessage message, int? numberOfRecords, CancellationToken cancellation);
        Task<AppMessage> PersistMessageAsync(AppMessage message, CancellationToken cancellation);
    }
}