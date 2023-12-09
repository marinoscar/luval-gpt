using Luval.Framework.Core.Configuration;
using Luval.GPT.Data;
using Luval.GPT.Data.MySql;
using Luval.GPT.GPT;
using Luval.GPT.GPT.OpenAI;
using Luval.GPT.Logging;
using Luval.GPT.Services;
using Luval.OpenAI;
using Luval.OpenAI.Chat;
using Luval.OpenAI.Models;
using Luval.WebGPT.Presenter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
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

        public static IServiceCollection AddDbContext(this IServiceCollection s)
        {
            var conn = Debugger.IsAttached ? ConfigManager.Get("DbConnection") : ConfigManager.Get("ProdDbConnection");
            var db = new MySqlAppDbContext(conn);
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
            return s;
        }

        public static IServiceCollection AddServices(this IServiceCollection s)
        {
            var openAIKey = ConfigManager.Get("OpenAIKey");
            var key = new NetworkCredential("", openAIKey).SecurePassword;
            s.AddScoped<ChatEndpoint>((s) => ChatEndpoint.CreateOpenAI(new ApiAuthentication(key), Model.GPTTurbo16k));
            s.AddScoped<IChatAgent, OpenAIChatAgent>();
            s.AddScoped<PromptAgentService>();
            s.AddScoped<PushAgentGptManager>();
            return s;
        }
    }
}
