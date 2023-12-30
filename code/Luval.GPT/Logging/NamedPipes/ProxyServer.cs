using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Logging.NamedPipes
{
    public class ProxyServer : IDisposable
    {
        public string PipeName { get; private set; }
        private H.Pipes.PipeServer<PipeLogEvent> _server;

        public ProxyServer(string? pipeName)
        {
            PipeName = pipeName ?? throw new ArgumentNullException(nameof(pipeName));
            _server = new H.Pipes.PipeServer<PipeLogEvent>(PipeName, new H.Formatters.NewtonsoftJsonFormatter());
        }

        public void Start()
        {
            _server.StartAsync().GetAwaiter().GetResult();
        }

        public void SendEvent(PipeLogEvent logEvent)
        {
            if (_server == null) throw new Exception("Pipe server not initialized");
            foreach (var c in _server.ConnectedClients)
                if (c.IsConnected)
                    c.WriteAsync(logEvent).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            if (_server != null)
                _server.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
