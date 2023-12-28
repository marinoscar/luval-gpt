using Luval.Framework.Core.Cache;
using Luval.Framework.Core.Configuration;
using Luval.GPT.Channels.Push;
using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
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
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
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
            var privateConfig = new JsonFileConfigurationProvider(new FileInfo(Path.Combine(GetRootFolderBasedOnOS(), "private.json")));
            var publicConfig = new JsonFileConfigurationProvider(new FileInfo(Path.Combine(GetRootFolderBasedOnOS(), "config.json")));
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

        public static IServiceCollection AddCacheProviders(this IServiceCollection s)
        {
            s.AddSingleton<ICacheProvider<string, AppUser>>(
                new CacheProvider<string, AppUser>("Users", new TimeExpirationPolicyFactory(TimeSpan.FromMinutes(45)), 
                new StaticMemoryCacheStorage<string, AppUser>()
                ));

            return s;
        }

        public static IServiceCollection AddGoogleAuth(this IServiceCollection s)
        {
            if (!ConfigManager.IsInitialized()) throw new Exception($"An instance of {typeof(ConfigManager)} needs to be initialized");

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

                    opt.CallbackPath = "/signin-google"; 

                    opt.Events.OnCreatingTicket = context =>
                    {
                        context?.Identity?.AddClaim(new Claim("providerName", "Google"));
                        CheckForAdmin(context?.Identity);
                        return Task.CompletedTask;
                    };
                });

            s.Configure<ForwardedHeadersOptions>(options => {
                // options.RequireHeaderSymmetry = false;

                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                // Only loopback proxies are allowed by default. Clear that restriction because forwarders are
                // being enabled by explicit configuration.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });


            return s;
        }

        public static AppDbContext CreateAppDbContext()
        {
            var conn = GetConnectionString();
            return new MySqlAppDbContext(conn);
        }

        public static string GetConnectionString()
        {
            return Debugger.IsAttached ? ConfigManager.Get("DbConnection") : ConfigManager.Get("ProdDbConnection");
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
            s.AddScoped<SqlPresenter>();
            return s;
        }

        private static void CheckForAdmin(ClaimsIdentity identity)
        {
            if (identity == null) return;
            if (!identity.Claims.Any()) return;
            if (!identity.HasClaim(i => i.Type == ClaimTypes.Email)) return;

            var email = identity.Claims.FirstOrDefault(i => i.Type == ClaimTypes.Email)?.Value;

            if (email == null) return;

            var validEmails = ConfigManager.GetOrDefault("AdminEmails", "")
                .Split(';').Select(i => i.ToLowerInvariant().Trim());

            if (validEmails.Any() && validEmails.Contains(email.ToLowerInvariant().Trim()))
                identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));

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

        private static string GetRootFolderBasedOnOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new FileInfo(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName).DirectoryName;
            else
            {
                return "/var/www/gpt_marin_cr";
            }
        }

    }
}
