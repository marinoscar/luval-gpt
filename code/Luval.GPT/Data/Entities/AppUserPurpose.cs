using Luval.Framework.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Data.Entities
{
    public class AppUserPurpose : IdentityEntity
    {
        [Required, MaxLength(50)]
        public string? AppUserId { get; set; }
        [Required]
        public string? Purpose { get; set; }

    }
}
