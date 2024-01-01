using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.BlobStorage
{
    public class BlobResult
    {
        public BlobResult(Blob blob)
        {
                Blob = blob ?? throw new ArgumentNullException(nameof(blob));
        }
        public Blob Blob { get; private set; }
        public string? ObjectUrl { get; set; }

    }
}
