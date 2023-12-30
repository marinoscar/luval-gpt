using Luval.Framework.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPush;

namespace Luval.GPT.Utilities
{
    public static class Utils
    {
        public static VapidDetails CreateVapid()
        {
            return new VapidDetails() { 
                Subject = ConfigManager.Get("VAPISubject"),
                PublicKey = ConfigManager.Get("VAPIKey"),
                PrivateKey = ConfigManager.Get("VAPISecret"),
            };
        }

        public static string GetAppUrl()
        {
            if (Debugger.IsAttached) return ConfigManager.Get("AppUrl_Dev");
            return ConfigManager.Get("AppUrl_Prod");
        }
    }
}
