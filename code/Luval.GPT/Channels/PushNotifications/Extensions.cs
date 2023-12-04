using Amazon.Runtime.Internal.Transform;
using Luval.GPT.Channels.PushNotifications.Models;
using Luval.GPT.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account.Usage.Record;

namespace Luval.GPT.Channels.PushNotifications
{
    public static class Extensions
    {
        public static  NotificationOptions ToNotificationOptions(this PushAgentMessage m)
        {
            return new NotificationOptions() { 
                Body = m.Title,
                Icon = m.AgentImageUrl,
                Image = m.MessageImageUrl,
                Silent = false,
                Vibrate = new[] { 200, 100, 200},
                Actions = new List<NotificationAction> { 
                    new() { Action = "explore", Title = m.CallToAction },
                    new() { Action = "close", Title = "Close" }
                },
                Data = new Dictionary<string, object> { 
                    { "navigateTo", m.AppRootUrl + m.Id },
                    { "dateOfArrival", DateTime.Now.ToString("o") }
                }
            };
        }
    }
}
