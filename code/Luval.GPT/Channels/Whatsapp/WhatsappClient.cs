using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Luval.GPT.Channels.Whatsapp
{
    public class WhatsappClient : IMessageClient
    {
        private readonly string _accountSid;
        private readonly string _accountToken;
        private readonly string _accountPhoneNumber;

        public WhatsappClient(string accountSid, string accountToken, string accountPhoneNumber)
        {
            _accountSid = accountSid;
            _accountToken = accountToken;
            _accountPhoneNumber = accountPhoneNumber;
            TwilioClient.Init(_accountSid, _accountToken);
        }

        public Task<ChannelMessageResponse> SendTextMessageAsync(string senderId, string messageBody, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var message = MessageResource.Create(
                body: messageBody,
                from: new Twilio.Types.PhoneNumber($"whatsapp:{_accountPhoneNumber}"),
                to: new Twilio.Types.PhoneNumber($"whatsapp:{senderId}"));
                return message.Convert();
            }, cancellationToken);
        }




    }
}
