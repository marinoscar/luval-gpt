using Luval.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPush;

namespace Luval.GPT.Data.Entities
{
    [Index(nameof(AppUserId))]
    public class PushAgentSubscription : IdentityEntity
    {
        [Required]
        public ulong? PushAgentId { get; set; }
        
        [Required, MaxLength(50)]
        public string? AppUserId { get; set; }

    }
}
