using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.AWS.Logger;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Luval.GPT.Logging
{
    public class AppLogger : Microsoft.Extensions.Logging.ILogger
    {

        private readonly Logger _logger;
        public AppLogger(Logger logger)
        {
            _logger = logger;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return _logger.IsEnabled(Map(logLevel));
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            _logger.Log(Map(logLevel), exception, formatter(state, exception));
        }

        private NLog.LogLevel Map(Microsoft.Extensions.Logging.LogLevel level)
        {
            switch (level)
            {
                case Microsoft.Extensions.Logging.LogLevel.Debug: return NLog.LogLevel.Debug;
                case Microsoft.Extensions.Logging.LogLevel.Warning: return NLog.LogLevel.Warn;
                case Microsoft.Extensions.Logging.LogLevel.Information: return NLog.LogLevel.Info;
                case Microsoft.Extensions.Logging.LogLevel.Trace: return NLog.LogLevel.Trace;
                case Microsoft.Extensions.Logging.LogLevel.Error: return NLog.LogLevel.Error;
                case Microsoft.Extensions.Logging.LogLevel.Critical: return NLog.LogLevel.Fatal;
                default: return NLog.LogLevel.Info;
            }
        }

        public static ILogger CreateWithFileAndConsole(NLog.LogLevel? minLevel, NLog.LogLevel? maxLevel)
        {
            return CreateLogger(minLevel, maxLevel, (c, min, max) => {
                AddConsoleTarget(c, min, max);
                AddFileTarget(c, min, max);
            });
        }

        public static ILogger CreateWithFileAndConsoleAndAws(string key, string secret, NLog.LogLevel? minLevel, NLog.LogLevel? maxLevel)
        {
            return CreateLogger(minLevel, maxLevel, (c, min, max) => {
                AddConsoleTarget(c, min, max);
                AddFileTarget(c, min, max);
                AddAwsTarget(c, key, secret, min, max);
            });
        }

        public static ILogger CreateWithConsoleAndAws(string key, string secret, NLog.LogLevel? minLevel, NLog.LogLevel? maxLevel)
        {
            return CreateLogger(minLevel, maxLevel, (c, min, max) => {
                AddConsoleTarget(c, min, max);
                AddAwsTarget(c, key, secret, min, max);
            });
        }

        public static Target AddConsoleTarget(LoggingConfiguration config, NLog.LogLevel? minLevel, NLog.LogLevel? maxLevel)
        {
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            AssignTarget(config, logconsole, minLevel, maxLevel);
            return logconsole;
        }

        public static Target AddAwsTarget(LoggingConfiguration config, string key, string secret, NLog.LogLevel? minLevel, NLog.LogLevel? maxLevel)
        {
            var awsTarget = new AWSTarget()
            {
                LogGroup = "LuvalGPT.Logs",
                Region = "us-east-1"
            };
            awsTarget.Credentials = new BasicAWSCredentials(key, secret);
            //config.AddTarget("aws", awsTarget);
            AssignTarget(config, awsTarget, minLevel, maxLevel);
            return awsTarget;
        }

        public static Target AddFileTarget(LoggingConfiguration config, NLog.LogLevel? minLevel, NLog.LogLevel? maxLevel)
        {
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "log.txt" };
            AssignTarget(config, logfile, minLevel, maxLevel);
            return logfile;
        }

        private static void AssignTarget(LoggingConfiguration config, Target target, NLog.LogLevel minLevel, NLog.LogLevel maxLevel)
        {
            NLog.LogLevel localMin;
            NLog.LogLevel localMax;
            if (minLevel == null) localMin = NLog.LogLevel.Debug; else localMin = minLevel;
            if (maxLevel == null) localMax = NLog.LogLevel.Fatal; else localMax = maxLevel;
            config.AddRule(localMin, localMax, target);
        }

        public static ILogger CreateLogger(NLog.LogLevel? minLevel, NLog.LogLevel? maxLevel, Action<LoggingConfiguration, NLog.LogLevel?, NLog.LogLevel?> action)
        {
            var config = new NLog.Config.LoggingConfiguration();
            action(config, minLevel, maxLevel);
            NLog.LogManager.Configuration = config;
            var logger = LogManager.GetCurrentClassLogger();
            return new AppLogger(logger);
        }
    }
}
