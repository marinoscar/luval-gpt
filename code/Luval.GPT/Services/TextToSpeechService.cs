using Luval.Framework.Services;
using Luval.GPT.GPT;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Services
{
    public class TextToSpeechService : LuvalServiceBase<string, Stream>
    {
        private readonly ITextToSpeechAgent _textToSpeechAgent;

        public TextToSpeechService(ILogger logger, ITextToSpeechAgent textToSpeechAgent) : base(logger, nameof(TextToSpeechService), new ServiceConfiguration() { NumberOfRetries = 3 })
        {
            _textToSpeechAgent = textToSpeechAgent ?? throw new ArgumentNullException(nameof(textToSpeechAgent));
        }

        protected override async Task<ServiceResponse<Stream>> DoExecuteAsync(string input, CancellationToken cancellationToken)
        {
            var stream = await _textToSpeechAgent.CreateAudioStreamAsync(input);
            return new ServiceResponse<Stream>()
            {
                Exception = null,
                Result = stream,
                Status = ServiceStatus.Completed,
                Message = "Success"
            };
        }
    }
}
