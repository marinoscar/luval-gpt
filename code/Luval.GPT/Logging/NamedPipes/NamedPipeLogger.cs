using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Logging.NamedPipes
{
    public class NamedPipeLogger : ILogger, IDisposable
    {

        public string PipeName { get; private set; }
        public PipeServer ProxyServer { get; private set; }

        public NamedPipeLogger() : this("marinlogpipe")
        {
        }

        public NamedPipeLogger(string pipeName)
        {
            PipeName = pipeName ?? throw new ArgumentNullException(nameof(pipeName));
            ProxyServer = new PipeServer(PipeName);
            ProxyServer.Start();
            Task.Delay(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var item = DoFormat(logLevel, eventId, exception, formatter(state, exception));
            ProxyServer?.SendEvent(new PipeLogEvent()
            {
                UtcDateTime = DateTime.UtcNow,
                EventType = Enum.GetName(typeof(LogLevel), logLevel),
                Message = item
            });
        }

        protected virtual string DoFormat(LogLevel logLevel, EventId eventId, Exception? exception, string message)
        {
            return exception == null ? $"{DateTime.UtcNow:s} [{logLevel}] ({eventId}) {message}"
                : $"{DateTime.UtcNow:s} [{logLevel}] ({eventId}) {exception}";
        }

        public void Dispose()
        {
            ProxyServer?.Dispose(); 
        }
    }
}
