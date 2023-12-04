using Luval.GPT.Data.Entities;

namespace Luval.GPT.GPT
{
    public interface IChatAgent
    {
        Task<AppMessage> ProcessPromptWithHistoryAsync(AppMessage message, int? historyCount, CancellationToken cancellationToken);
        Task<AppMessage> ProcessPrompt(AppMessage newMessage, IEnumerable<AppMessage> history, CancellationToken cancellationToken);
    }
}