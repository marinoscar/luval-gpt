using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.AWS.Logger;
using NLog.Config;
using NLog.Targets;
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

        public static IServiceCollection AddLogging(this IServiceCollection services, BasicAWSCredentials credentials)
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            //var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            var aws = GetAWSTarget(config, credentials);

            // Rules for mapping loggers to targets            
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logconsole);
            //config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logfile);

            //Send config
            return AddLogging(services, config);

        }

        private static AWSTarget GetAWSTarget(LoggingConfiguration config, BasicAWSCredentials credentials)
        {
            var awsTarget = new AWSTarget()
            {
                LogGroup = "LuvalGPT.Logs",
                Region = "us-east-1"
            };
            awsTarget.Credentials = credentials;
            //config.AddTarget("aws", awsTarget);
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, awsTarget);
            return awsTarget;
        }


    }
}
