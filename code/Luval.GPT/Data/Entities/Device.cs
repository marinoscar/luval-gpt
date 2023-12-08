using Luval.Framework.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Data.Entities
{
    public class Device : IdentityEntity
    {
        public string AppUserId { get; set; }

        [Required]
        public string Endpoint { get; set; }
        [Required]
        public string P256DH { get; set; }
        [Required]
        public string Auth { get; set; }

        public string? DeviceInformation { get; set; }
    }
}
