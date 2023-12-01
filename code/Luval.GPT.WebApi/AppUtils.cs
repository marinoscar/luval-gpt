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

        internal static ILogger GetLogger()
        {
            ILogger logger = null;
            if (Debugger.IsAttached) logger = AppLogger.CreateWithFileAndConsoleAndAws(ConfigManager.Get("AWSAccessKey"), ConfigManager.Get("AWSAccessSecret"), null, null);
            else AppLogger.CreateWithConsoleAndAws(ConfigManager.Get("AWSAccessKey"), ConfigManager.Get("AWSAccessSecret"), null, null);
            return logger;
        }

        internal static IMessageClient GetMessageClient()
        {
            var whatsapp = new WhatsappClient(ConfigManager.Get("TwilioSid"), ConfigManager.Get("TwilioSecret"), ConfigManager.Get("TwilioNumber"));
            return whatsapp;
        }

        internal static ChatEndpoint GetChatEndpoint()
        {
            var auth = new ApiAuthentication(new NetworkCredential("", ConfigManager.Get("OpenAIKey")).SecurePassword);
            return ChatEndpoint.CreateOpenAI(auth, Model.GPTTurbo16k);
        }

        internal static AppRepository GetAndInitAppRepo()
        {
            var conn = Debugger.IsAttached ? ConfigManager.Get("DbConnection") : ConfigManager.Get("ProdDbConnection");
            var db = new MySqlAppDbContext(conn);
            if (!db.Database.CanConnect()) throw new Exception("Unable to connect to the database");
            db.Database.EnsureCreated();
            var r = db.SeedDataAsync().Result;
            return new AppRepository(db);
        }
    }
}
