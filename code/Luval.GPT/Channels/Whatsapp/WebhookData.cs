using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Channels.Whatsapp
{
    public class WebhookData
    {
        public string? MediaContentType0 { get; set; }
        public string? SmsMessageSid { get; set; }
        public string? NumMedia { get; set; }
        public string? ProfileName { get; set; }
        public string? SmsSid { get; set; }
        public string? WaId { get; set; }
        public string? SmsStatus { get; set; }
        public string? Body { get; set; }
        public string? To { get; set; }
        public string? NumSegments { get; set; }
        public string? ReferralNumMedia { get; set; }
        public string? MessageSid { get; set; }
        public string? AccountSid { get; set; }
        public string? From { get; set; }
        public string? MediaUrl0 { get; set; }
        public string? ApiVersion { get; set; }

        public static WebhookData FromHttp(Dictionary<string, string?> data)
        {
            return new WebhookData()
            {
                AccountSid = GetVal(nameof(AccountSid), data),
                ApiVersion = GetVal(nameof(ApiVersion), data),
                Body = GetVal(nameof(Body), data),
                From = GetVal(nameof(From), data),
                MediaContentType0 = GetVal(nameof(MediaContentType0), data),
                MediaUrl0 = GetVal(nameof(MediaUrl0), data),
                MessageSid = GetVal(nameof(MessageSid), data),
                NumMedia = GetVal(nameof(NumMedia), data),
                NumSegments = GetVal(nameof(NumSegments), data),
                ProfileName = GetVal(nameof(ProfileName), data),
                ReferralNumMedia = GetVal(nameof(ReferralNumMedia), data),
                SmsMessageSid = GetVal(nameof(SmsMessageSid), data),
                SmsSid = GetVal(nameof(SmsSid), data),
                SmsStatus = GetVal(nameof(SmsStatus), data),
                To = GetVal(nameof(To), data),
                WaId = GetVal(nameof(WaId), data)
            };
        }

        private static string? GetVal(string k, Dictionary<string, string?> d)
        {
            return d.ContainsKey(k) ? d[k] : null;
        }
    }


}
