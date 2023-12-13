using Luval.Framework.Core;
using Luval.Framework.Core.Configuration;
using Newtonsoft.Json;

namespace Luval.WebGPT.Data.ViewModel
{
    public class ValidationToken
    {

        private DateTime _base = new DateTime(1983, 1, 19);

        public ValidationToken(string userId)
        {
            UserId = userId;
            Sequence = DateTime.UtcNow.Subtract(_base).TotalMilliseconds;
        }
        public double Sequence { get; set; }
        public string UserId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string Encrypt()
        {
            return ToString().Encrypt(ConfigManager.Get("EncryptionKey"));
        }

        public static ValidationToken Decript(string data)
        {
            var val = data.Decrypt(ConfigManager.Get("EncryptionKey"));
            return JsonConvert.DeserializeObject<ValidationToken>(val);
        }

        public bool ValidateSquence(DateTime dt)
        {
            var date = _base.AddMilliseconds(Sequence);
            return dt.Subtract(date).TotalHours <= 4;
        }


    }
}
