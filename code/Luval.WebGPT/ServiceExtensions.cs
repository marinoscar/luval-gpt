using Luval.Framework.Core.Configuration;
using Luval.GPT.Channels.Push;
using Luval.GPT.Data;
using Luval.GPT.Data.MySql;
using Luval.GPT.GPT;
using Luval.GPT.GPT.OpenAI;
using Luval.GPT.Logging;
using Luval.GPT.Services;
using Luval.GPT.Utilities;
using Luval.OpenAI;
using Luval.OpenAI.Chat;
using Luval.OpenAI.Models;
using Luval.WebGPT.Presenter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using IConfigurationProvider = Luval.Framework.Core.Configuration.IConfigurationProvider;

namespace Luval.WebGPT
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds an instance of <see cref="Luval.Framework.Core.Configuration.IConfigurationProvider"/>
        /// </summary>
        public static IServiceCollection AddConfing(this IServiceCollection s)
        {
            var privateConfig = JsonFileConfigurationProvider.LoadOrCreate("private", null, false);
            var publicConfig = JsonFileConfigurationProvider.LoadOrCreate("config", null, false);
            return AddConfing(s, privateConfig, publicConfig);
        }

        /// <summary>
        /// Adds an instance of <see cref="Luval.Framework.Core.Configuration.IConfigurationProvider"/>
        /// </summary>
        public static IServiceCollection AddConfing(this IServiceCollection s, params IConfigurationProvider[] configs)
        {
            var config = new Framework.Core.Configuration.ConfigurationProvider(configs);
            ConfigManager.Init(config);
            return s.AddSingleton<Luval.Framework.Core.Configuration.IConfigurationProvider>(config);
        }

        public static IServiceCollection AddLogger(this IServiceCollection s)
        {
            if (!ConfigManager.IsInitialized()) throw new Exception($"An instance of {typeof(ConfigManager)} this to be initialized");

            ILogger logger = null;
            if (Debugger.IsAttached) logger = AppLogger.CreateWithFileAndConsoleAndAws(ConfigManager.Get("AWSAccessKey"), ConfigManager.Get("AWSAccessSecret"), null, null);
            else logger = AppLogger.CreateWithConsoleAndAws(ConfigManager.Get("AWSAccessKey"), ConfigManager.Get("AWSAccessSecret"), null, null);
            s.AddSingleton<ILogger>(logger);
            return s;
        }

        public static IServiceCollection AddGoogleAuth(this IServiceCollection s)
        {
            if (!ConfigManager.IsInitialized()) throw new Exception($"An instance of {typeof(ConfigManager)} this to be initialized");
            s.AddAuthentication("Cookies")
                .AddCookie(opt =>
                {
                    opt.Cookie.Name = "GoogleOauth";
                    opt.LoginPath = "/auth/google-login";
                }).AddGoogle(opt =>
                {
                    opt.ClientId = ConfigManager.Get("GoogleAuthId");
                    opt.ClientSecret = ConfigManager.Get("GoogleAuthSecret");
                    opt.ClaimActions.MapJsonKey("urn:google:profile", "link");
                    opt.ClaimActions.MapJsonKey("urn:google:image", "picture");
                    //opt.CallbackPath = "/auth/google-login"; 
                    opt.Events.OnCreatingTicket = context =>
                    {
                        context?.Identity?.AddClaim(new Claim("providerName", "Google"));
                        return Task.CompletedTask;
                    };
                });

            return s;
        }

        public static AppDbContext CreateAppDbContext()
        {
            var conn = Debugger.IsAttached ? ConfigManager.Get("DbConnection") : ConfigManager.Get("ProdDbConnection");
            return new MySqlAppDbContext(conn);
        }

        public static IServiceCollection AddDbContext(this IServiceCollection s)
        {
            var db = CreateAppDbContext();
            if (!db.Database.CanConnect()) throw new Exception("Unable to connect to the database");
            db.Database.EnsureCreated();
            var r = db.SeedDataAsync().Result;
            s.AddSingleton<IAppDbContext>(db);
            return s;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection s)
        {
            return s.AddScoped<IRepository, AppRepository>();
        }

        public static IServiceCollection AddPresenters(this IServiceCollection s)
        {
            s.AddScoped<NotificationPresenter>();
            s.AddScoped<AgentPresenter>();
            s.AddScoped<ControllerClientPresenter>();
            return s;
        }

        private static ChatEndpoint CreateChatEndpoint()
        {
            var openAIKey = ConfigManager.Get("OpenAIKey");
            var key = new NetworkCredential("", openAIKey).SecurePassword;
            return ChatEndpoint.CreateOpenAI(new ApiAuthentication(key), Model.GPTTurbo16k);
        }

        private static IChatAgent CreateChatAgent(IRepository repository)
        {
            return new OpenAIChatAgent(repository, CreateChatEndpoint());
        }

        public static IServiceCollection AddServices(this IServiceCollection s)
        {
            s.AddScoped<ChatEndpoint>((s) => CreateChatEndpoint());
            s.AddScoped<IChatAgent, OpenAIChatAgent>();
            s.AddScoped<PromptAgentService>();
            s.AddScoped<PushAgentGptManager>();
            return s;
        }

        public static IServiceCollection AddAppHostedServices(this IServiceCollection s)
        {
            s.AddHostedService<PushAgentChronService>((s) =>
            {
                return CreatePushChronService(s);
            });
            return s;
        }

        private static PushAgentChronService CreatePushChronService(IServiceProvider s)
        {
            var repo = new AppRepository(CreateAppDbContext());
            var gpt = new PushAgentGptManager(repo, new PromptAgentService(s.GetRequiredService<ILogger>(), CreateChatAgent(repo)));
            var push = new PushClient(s.GetRequiredService<ILogger>(), Utils.CreateVapid());
            var refreshInterval =  Convert.ToUInt32(ConfigManager.GetOrDefault("PushAgentChronRefresh", "30"));
            var tickInMinutes = Convert.ToInt32(ConfigManager.GetOrDefault("PushAgentChronTickInMinutes", "1"));
            var now = DateTime.UtcNow;
            var startOn = now.AddMinutes(1).Subtract(now);

            var cs = new PushAgentChronService(s.GetRequiredService<ILogger>(), repo,
                gpt, push, refreshInterval, startOn, TimeSpan.FromMinutes(tickInMinutes));
            return cs;
        }

    }
}
