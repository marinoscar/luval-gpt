using Luval.Framework.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Channels.Push.Models
{
    public class OptionActionModel
    {
        public string? Title { get; set; }
        public string? CallToAction { get; set; }
        public string? Decline { get; set; }
        public string? Banner { get; set; }

        public static OptionActionModel FromGpt(string agentText)
        {
            var json = agentText.GetTextInBetween("``");
            var options = JsonConvert.DeserializeObject<OptionActionModel>(json, new JsonSerializerSettings(){
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                NullValueHandling = NullValueHandling.Ignore,
            });
            return options;
        }
    }
}
