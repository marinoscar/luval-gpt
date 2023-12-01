using Luval.GPT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Data
{
    public class AppRepository : IAppRepository
    {

        private readonly AppDbContext _dbContext;

        public AppRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AppMessage> PersistMessageAsync(AppMessage message, CancellationToken cancellation)
        {
            await _dbContext.AppMessages.AddAsync(message, cancellation);
            await _dbContext.SaveChangesAsync(cancellation);
            return message;
        }
        public async Task<ApplicationUser?> GetApplicationUser(string providerName, string providerKey, CancellationToken cancellation)
        {
            return await _dbContext.ApplicationUsers.FirstOrDefaultAsync(i => i.ProviderName == providerName && i.ProviderKey == providerKey, cancellation);
        }

        public async Task<IEnumerable<AppMessage>> GetConversationHistory(AppMessage message, int? numberOfRecords, CancellationToken cancellation)
        {
            Func<AppMessage, bool> predicate = i => i.ProviderKey == message.ProviderKey && i.ProviderName == message.ProviderName && i.ChatType == message.ChatType;
            if (numberOfRecords == null) return await GetAllMessagesAsync(predicate, cancellation);

            var recordCount = await GetNumberofChatRecordsAsync(predicate, cancellation);

            var delta = recordCount - numberOfRecords;
            if (delta < recordCount) return await GetAllMessagesAsync(predicate, cancellation);

            return await Task.Run(() =>
            {
                return _dbContext.AppMessages.Where(predicate).Skip(delta.Value);
            }, cancellation);
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
