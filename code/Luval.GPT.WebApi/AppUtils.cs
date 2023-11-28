using Luval.Framework.Core.Configuration;
using Luval.GPT.Channels;
using Luval.GPT.Channels.Whatsapp;
using Luval.GPT.Data;
using Luval.GPT.Data.MySql;
using Luval.GPT.Logging;
using Luval.OpenAI;
using Luval.OpenAI.Chat;
using Luval.OpenAI.Models;
using System.Diagnostics;
using System.Net;
using IConfigurationProvider = Luval.Framework.Core.Configuration.IConfigurationProvider;

namespace Luval.GPT.WebApi
{
    internal class AppUtils
    {
        internal static IConfigurationProvider GetConfigurationProvider()
        {
            var privateConfig = JsonFileConfigurationProvider.LoadOrCreate("private", null, false);
            var publicConfig = JsonFileConfigurationProvider.LoadOrCreate("config", null, false);
            return new Framework.Core.Configuration.ConfigurationProvider(privateConfig, publicConfig);
        }

        internal static ILogger GetLogger(IConfigurationProvider config)
        {
            ILogger logger = null;
            if (Debugger.IsAttached) logger = AppLogger.CreateWithFileAndConsoleAndAws(config.Get("AWSAccessKey"), config.Get("AWSAccessSecret"), null, null);
            else AppLogger.CreateWithConsoleAndAws(config.Get("AWSAccessKey"), config.Get("AWSAccessSecret"), null, null);
            return logger;
        }

        internal static IMessageClient GetMessageClient(IConfigurationProvider config)
        {
            var whatsapp = new WhatsappClient(config.Get("TwilioSid"), config.Get("TwilioSecret"), config.Get("TwilioNumber"));
            return whatsapp;
        }

        internal static ChatEndpoint GetChatEndpoint(IConfigurationProvider config)
        {
            var auth = new ApiAuthentication(new NetworkCredential("", config.Get("OpenAIKey")).SecurePassword);
            return ChatEndpoint.CreateOpenAI(auth, Model.GPTTurbo16k);
        }

        internal static AppRepository GetAndInitAppRepo(IConfigurationProvider config)
        {
            var conn = Debugger.IsAttached ? config.Get("DbConnection") : config.Get("ProdDbConnection");
            var db = new MySqlAppDbContext(conn);
            db.Database.EnsureCreated();
            var r = db.SeedDataAsync().Result;
            return new AppRepository(db);
        }
    }
}
