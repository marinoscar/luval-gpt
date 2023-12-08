using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Luval.WebGPT.Data.ViewModel
{
    public class DeviceSubscription
    {
        public DeviceSubscription() : this(null)
        {
        }

        public DeviceSubscription(ClaimsPrincipal identity)
        {
            if (identity != null)
                User = identity.ToUser();
        }

        public string? ApplicationServerKey { get; set; }
        public WebUser? User { get; set; }
        public string? Endpoint { get; set; }
        public string? P256DH { get; set; }
        public string? Auth { get; set; }
    }
}
