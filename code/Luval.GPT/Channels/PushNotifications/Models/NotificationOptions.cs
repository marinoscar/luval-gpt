using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Channels.PushNotifications.Models
{
    public class NotificationOptions
    {
        // Visual Options
        public string? Body { get; set; }
        public string? Icon { get; set; }
        public string? Image { get; set; }
        public string? Badge { get; set; }
        public string? Dir { get; set; }
        public long? Timestamp { get; set; }

        // Both visual & behavioral options
        public List<NotificationAction>? Actions { get; set; }
        public Dictionary<string, object>? Data { get; set; }

        // Behavioral Options
        public string? Tag { get; set; }
        public bool? RequireInteraction { get; set; }
        public bool? Renotify { get; set; }
        public int[]? Vibrate { get; set; }
        public string? Sound { get; set; }
        public bool? Silent { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }
    }

    public class NotificationAction
    {
        public string? Action { get; set; }
        public string? Title { get; set; }
        public string? Icon { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }
    }
}
