using Luval.Framework.Core.Configuration;
using Luval.OpenAI;
using Luval.OpenAI.Audio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.GPT.OpenAI
{
    public class TextToSpeechAgent : ITextToSpeechAgent
    {
        private readonly TextToSpeechEndpoint _endpoint;


        public TextToSpeechAgent() : this(ConfigManager.Get("OpenAIKey"))
        {

        }
        public TextToSpeechAgent(string key)
        {
            _endpoint = new TextToSpeechEndpoint(new ApiAuthentication(key));
        }

        public async Task<Stream> CreateAudioStreamAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(nameof(text));

            var req = GetRequest(text);

            var res = await _endpoint.SendAsync(req);
            return new MemoryStream(res.ToArray());
        }

        private TextToSpeechRequest GetRequest(string input)
        {
            var defVal = "{\"model\":\"tts-1-hd\",\"voice\":\"onyx\",\"speed\":1}";
            var configValue = ConfigManager.GetOrDefault("OpenAIAudioConfig", defVal);
            var item = JsonConvert.DeserializeObject<TextToSpeechRequest>(configValue) ?? throw new Exception("Json format in config value OpenAIAudioConfig is invalid");
            item.Input = input;
            return item;
        }
    }
}
