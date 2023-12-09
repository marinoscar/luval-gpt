using Luval.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Data.Entities
{
    public class Agent : StringAuditEntry
    {
        [Required, MaxLength(50)]
        public string? Code { get; set; }
        [Required, MaxLength(150)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(25)]
        public string? Provider { get; set; }
        [MaxLength(100)]
        public string? ProviderModel { get; set; }
        [MaxLength(50)]
        public string? ProviderModelVersion { get; set; }

        [Required]
        public string? SystemMessage { get; set; }
        public string? MessagePrefix { get; set; }
        public string? MessageSuffix { get; set; }
    }
}
