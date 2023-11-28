using Luval.GPT.Data.Entities;

namespace Luval.GPT.GPT
{
    public interface IChatAgent
    {
        Task<AppMessage> ProcessPromptAsync(AppMessage message, int? historyCount, CancellationToken cancellationToken);
    }
}