using Microsoft.AspNetCore.SignalR;
using NLog.SignalR;

namespace Luval.WebGPT.Hubs
{
    public class LoggingHub : Hub<ILoggingHub>
    {
        public void Log(LogEvent logEvent)
        {
            Clients.Others.Log(logEvent);
        }
    }
}
