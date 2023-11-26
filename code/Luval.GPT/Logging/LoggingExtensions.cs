using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Logging
{
    public static class LoggingExtensions
    {
        public static IServiceCollection AddLogging(this IServiceCollection services, Logger logger)
        {
            services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(new AppLogger(logger));
            return services;
        }

        public static IServiceCollection AddLogging(this IServiceCollection services, LoggingConfiguration config)
        {
            // Apply config           
            NLog.LogManager.Configuration = config;
            return AddLogging(services, NLog.LogManager.GetCurrentClassLogger());
        }

        public static IServiceCollection AddLogging(this IServiceCollection services)
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logconsole);
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logfile);

            //Send config
            return AddLogging(services, config);

        }
    }
}
