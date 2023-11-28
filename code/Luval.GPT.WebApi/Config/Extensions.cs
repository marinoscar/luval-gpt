using Luval.Framework.Core.Configuration;
using Microsoft.AspNetCore.Http;
using IConfigurationProvider = Luval.Framework.Core.Configuration.IConfigurationProvider;

namespace Luval.GPT.WebApi.Config
{
    public static class Extensions
    {
        public static Dictionary<string, string?> ToDictionary(this IFormCollection collection)
        {
            var dictionary = new Dictionary<string, string? >();

            foreach (var item in collection)
            {
                dictionary[item.Key] = item.Value.FirstOrDefault();
            }

            return dictionary;
        }
    }
}
