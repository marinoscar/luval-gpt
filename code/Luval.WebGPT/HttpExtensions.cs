using Luval.GPT.Data.Entities;
using Luval.WebGPT.Data.ViewModel;
using System.Security.Claims;

namespace Luval.WebGPT
{
    public static class HttpExtensions
    {
        public static WebUser ToUser(this ClaimsPrincipal identity)
        {
            return new WebUser()
            {
                ProviderName = identity.GetValue("providerName"),
                ProviderKey = identity.GetValue(ClaimTypes.NameIdentifier),
                DisplayName = identity.GetValue(ClaimTypes.GivenName),
                Email = identity.GetValue(ClaimTypes.Email),
                PictureUrl = identity.GetValue("picture")
            };
        }

        public static string? GetValue(this ClaimsPrincipal c, string type)
        {
            if (c == null) return null;
            if (!c.HasClaim(i => i.Type == type)) return null;
            return c.Claims.First(i => i.Type == type).Value;
        }

        public static string? GetBaseUrl(this HttpRequest req)
        {
            if (req == null) return null;
            var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }

            return uriBuilder.Uri.AbsoluteUri;
        }

        public static string? GetBaseUrl(this HttpContext context)
        {
            return GetBaseUrl(context.Request);
        }

        public static HttpClient CreateAppClient(this IHttpClientFactory factory, IHttpContextAccessor context)
        {
            var http = factory.CreateClient();
            http.BaseAddress = new Uri(context.HttpContext.GetBaseUrl());
            return http;
        }

        public static bool IsAuthenticated(this IHttpContextAccessor c)
        {
            if (c == null) return false;
            return c != null && c.HttpContext != null && c.HttpContext.User != null && c.HttpContext.User.Identity != null &&
                c.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
