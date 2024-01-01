using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.BlobStorage
{
    public class Blob
    {
        public Blob()
        {
            Id = Guid.NewGuid().ToString();
            Properties = new Dictionary<string, string>() { { "Id", Id } };
        }

        public string Id { get; set; }
        public string? Name { get; set; }
        public IDictionary<string, string> Properties { get; private set; }

        public Stream? Content { get; set; }
    }
}
