using Luval.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Data.Entities
{
    [Index(nameof(ProviderName)), Index(nameof(ProviderKey))]
    public class MessageAccount : StringAuditEntry
    {
        [Required, MaxLength(150)]
        public string? UserName { get; set; }
        [Required, MaxLength(150)]
        public string? ProviderName { get; set; }
        [Required, MaxLength(150)]
        public string? ProviderKey { get; set; }
    }
}
