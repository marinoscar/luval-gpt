using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Channels
{
    public class ChannelMessageResponse
    {
        public string? Status { get; set; }
        public string? ErrorMessage { get; set; }
        public string? FromUser { get; set; }
        public string? ToUser { get; set; }
        public string? Provider { get; set; }
        public string? MessageBody { get; set; }
        public DateTime? Date { get; set; }
    }
}
