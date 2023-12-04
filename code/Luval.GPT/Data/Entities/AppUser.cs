using Luval.Framework.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Data.Entities
{
    [Index(nameof(ProviderName)), Index(nameof(ProviderKey)), Index(nameof(Email))]
    public class AppUser : StringAuditEntry
    {
        public AppUser()
        {
            ProviderName = "Google";
            ProviderKey = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets or sets the name of the user provider i.e. Google, Microsoft, etc
        /// </summary>
        [Required, MaxLength(50)]
        public string? ProviderName { get; set; }
        /// <summary>
        /// Gets or sets the id on the user provider
        /// </summary>
        [Required, MaxLength(50)]
        public string? ProviderKey { get; set; }
        /// <summary>
        /// Email address
        /// </summary>
        [Required, MaxLength(100)]
        public string? Email { get; set; }
        [MaxLength(50)]
        public string? PhoneNumber { get; set; }
        [MaxLength(500)]
        public string? ProfilePicUrl { get; set; }
        public string? UserData { get; set; }
    }
}
