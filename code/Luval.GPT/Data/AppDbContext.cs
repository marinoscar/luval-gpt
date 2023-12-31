﻿using Luval.GPT.Channels;
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
    public class AppDbContext : DbContext, IAppDbContext
    {

        private const string AdminUserId = "ADMIN-USER";

        public DbSet<Agent> Agents { get; set; }
        public DbSet<MessageAccount> MessageAccounts { get; set; }
        public DbSet<AppMessage> AppMessages { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<PushAgent> PushAgents { get; set; }
        public DbSet<PushAgentSubscription> PushAgentSubscriptions { get; set; }

        public DbSet<Device> Devices { get; set; }

        public DbSet<AppUserPurpose> AppUserPurposes { get; set; }



        /// <summary>
        /// Sets the seed data for the data context
        /// </summary>
        /// <param name="cancellationToken"></param>
        public virtual async Task<int> SeedDataAsync(CancellationToken cancellationToken = default)
        {
            await InitAppUserAsync(cancellationToken);
            await InitAgentAsync(cancellationToken);
            await InitUsersAsync(cancellationToken);
            await InitPushAgentAsync(cancellationToken);
            await TaskInitAppUserPurpose(cancellationToken);
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

        private async Task InitAppUserAsync(CancellationToken cancellationToken = default)
        {
            if (AppUsers != null && !AppUsers.Any())
            {
                await AppUsers.AddAsync(new AppUser()
                {
                    Id = AdminUserId,
                    Email = "oscar@marin.cr",
                    ProviderName = "Google",
                    ProviderKey = "104617773614037529966",
                    ProfilePicUrl = "https://lh3.googleusercontent.com/a/ACg8ocJM3TGDkcQ39GZ_4a74fRT_T83b2QBvZsWhzgG8G9x_Xg=s96-c",
                    CreatedBy = AdminUserId,
                    UpdatedBy = AdminUserId
                });
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
                    ProviderModel = "gpt-3.5-turbo-16k-0613",
                    CreatedBy = AdminUserId,
                    UpdatedBy = AdminUserId,
                },
                cancellationToken);
            }
        }

        private async Task InitPushAgentAsync(CancellationToken cancellationToken = default)
        {
            if (PushAgents != null && !PushAgents.Any())
            {
                var agent =  
                await PushAgents.AddAsync(new PushAgent()
                {
                    RootMessage = GetRootHeaderMessage(),
                    UserPrompt = GetFollowUpMessage(),
                    Description = "Rev up your fitness journey with our AI-powered Gym Motivation feature. Tailored to your personal goals, it delivers inspiring messages right when you need them most. Schedule notifications to fit your routine, keeping you focused and energized for every workout. Stay inspired, stay fit!",
                    ChronExpression = "0 18 * * 1-4",
                    ChronExpressionPrompt = "",
                    Name = "Gym Motivation Agent",
                    ImageUrl = "https://raw.githubusercontent.com/marinoscar/luval-gpt/main/code/Luval.WebGPT/wwwroot/img/gym-icon.png",
                    IsPublic = true,
                    PromptSuffix = GetSuffix(),
                    AppUserId = AdminUserId,
                    Timezone = "Central Standard Time",
                    SystemMessage = "You are a helpful assistant",
                    PromptPrefix = string.Empty,
                    CreatedBy = AdminUserId,
                    UpdatedBy = AdminUserId 
                });
                await SaveChangesAsync(cancellationToken);
                if(PushAgentSubscriptions != null)
                {
                    await PushAgentSubscriptions.AddAsync(new PushAgentSubscription()
                    {
                        AppUserId = AdminUserId,
                        PushAgentId = agent.Entity.Id
                    }, cancellationToken);
                    await SaveChangesAsync(cancellationToken);
                }
            }
        }

        private async Task TaskInitAppUserPurpose(CancellationToken cancellationToken = default)
        {
            if (AppUserPurposes != null && !AppUserPurposes.Any())
            {
                await AppUserPurposes.AddAsync(new AppUserPurpose() { 
                    AppUserId = AdminUserId,
                    Purpose = GetPurpose()
                }, cancellationToken);
            }
        }

        private string GetFollowUpMessage()
        {
            return @"
Great! I need another message!
";
        }

        private string GetRootHeaderMessage()
        {
            return @"
I am looking to get motivated to go to the gym, I am not feeling like going right now, I need a message to motivate me to go to the gym
";
        }

        private string GetPurpose()
        {
            return @"
My purpose is to honor the life I've been given and the people I love – my daughters Lucy and Valeria, and my wife Pamela – by cultivating a healthier, stronger version of myself. Each day, I strive to nurture my body and mind, recognizing that my wellbeing is a gift to myself and those I cherish. In embracing my Catholic faith, I find strength and guidance, leading me towards a path of physical and spiritual wellness. My commitment to health is not just a personal goal but a testament to the values I hold dear – family, faith, and self-improvement. By staying active, eating healthily, and keeping my heart open to God's guidance, I am not only losing weight but also gaining a more fulfilling, vibrant life. This journey is about more than numbers on a scale; it's about living fully, loving deeply, and growing in faith and health each day.
";
        }

        private string GetSuffix()
        {
            return @"
* Start the message with ^^^
* Do not exceed 1000 characters
* Include a few emojis
* Remember that my name is Oscar
* Do not include any Hashatags
* End the message with ^^^
";
        }

    }
}
