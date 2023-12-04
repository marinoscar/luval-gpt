using Luval.WebPush.Models;
using WebPush;

namespace Luval.WebPush.Data
{
    public static class PersistentStorage
    {
        public static List<ClientSubscription> Data { get; set; } = new List<ClientSubscription>();

        public static List<string> GetClientNames()
        {
            return Data.Select(i => i.Client).ToList();
        }

        internal static PushSubscription GetSubscription(string client)
        {
            return Data.Single(i => i.Client == client).PushSubscription;
        }

        internal static void SaveSubscription(string client, PushSubscription subscription)
        {
            Data.Add(new ClientSubscription() { Client = client, PushSubscription = subscription });
        }
    }
}
