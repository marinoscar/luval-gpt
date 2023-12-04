using Luval.Framework.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Services
{
    public class ReminderChronService : ChronTimeHostedService
    {
        private readonly Action _run;

        public ReminderChronService(ILogger logger, string chronExpression, Action doWork, TimeSpan dueTime,  TimeSpan period) : base(logger, chronExpression, "Central Standard Time", dueTime, period)
        {
            _run = doWork;
        }

        protected override void DoWork()
        {
            _run();
            Logger.LogDebug("Process completed");
        }
    }
}
