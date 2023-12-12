using Luval.Framework.Core.Configuration;
using System;
using System.Collections.Generic;
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
    }
}
