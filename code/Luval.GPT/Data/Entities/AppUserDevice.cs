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
    public class AppUserDevice : IdentityEntity
    {
        public AppUserDevice()
        {
                ErrorCount = 0;
        }

        [Required, MaxLength(50)]
        public string? AppUserId { get; set; }

        [Required]
        public string? Endpoint { get; set; }
        [Required]
        public string? P256DH { get; set; }
        [Required]
        public string? Auth { get; set; }
        public string? DeviceInfo { get; set; }
        [Required]
        public bool HasError { get; set; }
        [Required]
        public uint ErrorCount { get; set; }

        public PushSubscription ToPushSub()
        {
            return new PushSubscription(Endpoint, P256DH, Auth);
        }
    }
}
