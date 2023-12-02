using Luval.Framework.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Services
{
    public class ScheduleGPTService : ChronTimeHostedService
    {
        public ScheduleGPTService(ILogger logger, string chronExpression, TimeSpan period) : base(logger, chronExpression, "Central Standard Time", period)
        {
        }

        protected override void DoWork()
        {
            Logger.LogInformation("SUCCESS!! TRIGERED");
        }
    }
}
