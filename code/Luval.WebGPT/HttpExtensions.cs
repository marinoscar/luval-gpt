using Luval.Framework.Core;
using Luval.Framework.Core.Configuration;
using Luval.GPT.Data.Entities;
using Luval.WebGPT.Data.ViewModel;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

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

        public static string? GetBaseUrl(this IHttpContextAccessor context)
        {
            return GetBaseUrl(context.HttpContext.Request);
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

        public static Task<HttpResponseMessage> PostJsonLocalAsync(this HttpClient c, string baseUrl, string route, string payload)
        {
            return SendJsonLocalAsync(c, baseUrl, route, HttpMethod.Post, payload);
        }

        public static Task<HttpResponseMessage> GetJsonLocalAsync(this HttpClient c, string baseUrl, string route, string payload)
        {
            return SendJsonLocalAsync(c, baseUrl, route, HttpMethod.Get, payload);
        }

        public static Task<HttpResponseMessage> SendJsonLocalAsync(this HttpClient c, string baseUrl, string route, HttpMethod method, string payload)
        {
            var key = DateTime.UtcNow.ToString().Encrypt(ConfigManager.Get("EncryptionKey"));
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            if (!string.IsNullOrEmpty(route) && route.EndsWith("/"))
            {
                route = route.Remove(route.Length - 1);
            }

            c.DefaultRequestHeaders.Add("Accept", "application/json");
            c.DefaultRequestHeaders.Add("AppToken", key);
            var requestUrl = new Uri(baseUrl + route);
            var request = new HttpRequestMessage()
            {
                Content = content,
                Method = method,
                RequestUri = requestUrl
            };

            return c.SendAsync(request);
        }
    }
}
