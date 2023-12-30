using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Logging.NamedPipes
{
    public class PipeLogEvent
    {
        public DateTime? UtcDateTime { get; set; }
        public string? Message { get; set; }
        public string? EventType { get; set; }
    }
}
