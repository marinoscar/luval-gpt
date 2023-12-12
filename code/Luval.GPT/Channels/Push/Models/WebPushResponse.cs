using Luval.GPT.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Channels.Push.Models
{
    public class WebPushResponse
    {
        public AppMessage? MessageContent { get; set; }
        public AppMessage? NotificationContent { get; set; }


        public NotificationOptions GetOptions(string webSite, string? icon)
        {
            var actionModel = OptionActionModel.FromGpt(NotificationContent.AgentText);
            return new NotificationOptions()
            {
                Body = actionModel.Title,
                Image = actionModel.Banner,
                Icon = icon,
                Silent = false,
                Vibrate = new[] { 200, 100, 200 },
                Data = new Dictionary<string, object> {
                    { "navigateTo", $"{webSite}/Message/{ MessageContent.Id }"}
                },
                Actions = new List<NotificationAction>() {
                    new() { Title = actionModel.CallToAction, Action = "explore" },
                    new() { Title = actionModel.Decline, Action = "close" }
                }
            };

        }
    }
}
