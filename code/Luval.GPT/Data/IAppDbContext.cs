using Luval.GPT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;

namespace Luval.GPT.Data
{
    public interface IAppDbContext
    {
        DbSet<Agent> Agents { get; set; }
        DbSet<AppMessage> AppMessages { get; set; }
        DbSet<AppUser> AppUsers { get; set; }
        DbSet<MessageAccount> MessageAccounts { get; set; }
        DbSet<PushAgent> PushAgents { get; set; }
        DbSet<PushAgentSubscription> PushAgentSubscriptions { get; set; }
        DbSet<Device> Devices { get; set; }

        Task<int> SeedDataAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        int SaveChanges();

        DatabaseFacade Database { get; }
    }
}