using Luval.GPT.Channels;
using Luval.GPT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Data
{
    public class AppDbContext : DbContext
    {

        public DbSet<Agent> Agents { get; set; }
        public DbSet<MessageAccount> MessageAccounts { get; set; }
        public DbSet<AppMessage> AppMessages { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppUserDevice> AppUserDevices { get; set; }
        public DbSet<PushAgent> PushAgents { get; set; }
        public DbSet<PushAgentMessage> PushAgentMessages { get; set; }
        public DbSet<PushAgentSubscription> PushAgentSubscriptions { get; set; }



        /// <summary>
        /// Sets the seed data for the data context
        /// </summary>
        /// <param name="cancellationToken"></param>
        public virtual async Task<int> SeedDataAsync(CancellationToken cancellationToken = default)
        {
            await InitAgentAsync(cancellationToken);
            await InitUsersAsync(cancellationToken);
            return await SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>()
                .HasIndex(c => c.Code).IsUnique();
        }


        private async Task InitUsersAsync(CancellationToken cancellationToken = default)
        {
            if (MessageAccounts != null && !MessageAccounts.Any())
            {
                await MessageAccounts.AddAsync(new MessageAccount() { UserName = "Oscar Marin", ProviderName = ChannelProviders.Whatsapp, ProviderKey = "+12488057580" }, cancellationToken);
                await MessageAccounts.AddAsync(new MessageAccount() { UserName = "Oscar Marin", ProviderName = ChannelProviders.SMS, ProviderKey = "+12488057580" }, cancellationToken);
                await MessageAccounts.AddAsync(new MessageAccount() { UserName = "Oscar Marin", ProviderName = ChannelProviders.Telegram, ProviderKey = "5640988132" }, cancellationToken);
            }
        }

        private async Task InitAgentAsync(CancellationToken cancellationToken = default)
        {
            if (Agents != null && !Agents.Any())
            {
                await Agents.AddAsync(new Agent()
                {
                    Code = "EmpowerGPT",
                    Name = "Empower GPT",
                    SystemMessage = "You are a helpful assistant",
                    Provider = "OpenAI",
                    ProviderModel = "gpt-3.5-turbo-16k-0613"
                },
                cancellationToken);
            }
        }

    }
}
