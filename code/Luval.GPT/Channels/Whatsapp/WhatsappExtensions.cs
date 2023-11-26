using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace Luval.GPT.Channels.Whatsapp
{
    public static class WhatsappExtensions
    {
        public static ChannelMessageResponse Convert(this MessageResource m)
        {
            return new ChannelMessageResponse()
            {
                Date = m.DateSent,
                ErrorMessage = m.ErrorMessage,
                Status = m.Status.ToString(),
                FromUser = m.From.ToString(),
                ToUser = m.To.ToString(),
                MessageBody = m.Body,
                Provider = "Whatsapp"
            };
        }
    }
}
