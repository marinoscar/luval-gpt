using WebPush;

namespace Luval.WebPush.Models
{
    public class ClientSubscription
    {
        public string Client { get; set; }
        public PushSubscription PushSubscription { get; set; }
    }
}
