using Luval.Framework.Core.Configuration;
using Luval.GPT.Logging;
using System.Diagnostics;
using IConfigurationProvider = Luval.Framework.Core.Configuration.IConfigurationProvider;

namespace Luval.GPT.WebApi
{
    internal class AppUtils
    {
        internal static IConfigurationProvider GetConfigurationProvider()
        {
            var privateConfig = new JsonFileConfigurationProvider("private.json");
            var publicConfig = new JsonFileConfigurationProvider("config.json");
            return new Framework.Core.Configuration.ConfigurationProvider(privateConfig, publicConfig);
        }

        internal static ILogger GetLogger(IConfigurationProvider config)
        {
            ILogger logger = null;
            if (Debugger.IsAttached) logger = AppLogger.CreateWithFileAndConsoleAndAws(config.Get("AWSAccessKey"), config.Get("AWSAccessSecret"), null, null);
            else AppLogger.CreateWithConsoleAndAws(config.Get("AWSAccessKey"), config.Get("AWSAccessSecret"), null, null);
            return logger;
        }
    }
}
