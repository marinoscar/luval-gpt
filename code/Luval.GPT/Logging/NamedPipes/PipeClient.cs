using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Logging.NamedPipes
{
    public class PipeClient : IDisposable
    {
        public string PipeName { get; private set; }
        private H.Pipes.PipeClient<PipeLogEvent> _client;

        public Action<PipeLogEvent?>? OnMessageRecieved { get; set; }

        public PipeClient() : this("marinlogpipe")
        {
        }

        public PipeClient(string pipeName)
        {
            PipeName = pipeName ?? throw new ArgumentNullException(nameof(pipeName));
            _client = new H.Pipes.PipeClient<PipeLogEvent>(PipeName, formatter: new H.Formatters.NewtonsoftJsonFormatter());
            _client.MessageReceived += MessageReceived;
        }

        private void MessageReceived(object? sender, H.Pipes.Args.ConnectionMessageEventArgs<PipeLogEvent?> e)
        {
            if (OnMessageRecieved == null) return;
            OnMessageRecieved(e.Message);
        }

        public void OpenConnection()
        {
            if (_client.IsConnected) return;

            _client.ConnectAsync().GetAwaiter().GetResult();
        }

        public void CloseConnection()
        {
            _client.DisconnectAsync().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            if (_client != null)
                _client.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
