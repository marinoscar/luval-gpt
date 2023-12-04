using Luval.Framework.Core.Configuration;
using Luval.GPT.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Diagnostics;
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
                        var pic = context.User.GetProperty("picture").GetString();
                        context?.Identity?.AddClaim(new Claim("picture", pic));
                        return Task.CompletedTask;
                    };
                });

            return s;
        }
    }
}
