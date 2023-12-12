using Luval.GPT.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebPush;

namespace Luval.GPT.Utilities
{
    public static class Extensions
    {
        public static AppUser ToUser(this ClaimsPrincipal p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            if (p.Claims.GetValue("providerName") == null) throw new ArgumentException("providerName claim missing");
            return new AppUser()
            {
                ProviderKey = p.Claims.GetValue(ClaimTypes.NameIdentifier),
                ProviderName = p.Claims.GetValue("providerName"),
                Email = p.Claims.GetValue(ClaimTypes.Email),
                ProfilePicUrl = p.Claims.GetValue("link"),
            };
        }

        public static string? GetValue(this IEnumerable<Claim> claims, string type)
        {
            var c = claims.FirstOrDefault(c => c.Type == type);
            if (c == null) return null;
            return c.Value;
        }
    }
}
