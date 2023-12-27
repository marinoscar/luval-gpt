using Luval.Framework.Core.Configuration;
using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using Luval.WebGPT.Data.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using WebPush;

namespace Luval.WebGPT.Presenter
{
    public class NotificationPresenter
    {

        private readonly ILogger _logger;
        private readonly IRepository _repository;


        public NotificationPresenter(ILogger logger, IRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Sub = new DeviceSubscription()
            {
                ApplicationServerKey = ConfigManager.Get("VAPIKey"),
                User = new WebUser()
            };
        }

        public DeviceSubscription Sub { get; set; }
        public IJSRuntime Js { get; set; }

        public void OnAuthFieldChanged(ChangeEventArgs e)
        {
            Sub.Auth = Convert.ToString(e.Value);
        }

        public void OnP256FieldChanged(ChangeEventArgs e)
        {
            Sub.P256DH = Convert.ToString(e.Value);
        }

        public void OnEndpointFieldChanged(ChangeEventArgs e)
        {
            Sub.Endpoint = Convert.ToString(e.Value);
        }

        public async Task RegisterDevice()
        {
            try
            {
                if (Sub == null) throw new ArgumentNullException(nameof(Sub));
                if (Sub.User == null) throw new ArgumentException(nameof(Sub.User));

                var res = await Js.InvokeAsync<PushSubscription>("returnVAPIInfo");
                Sub.Auth = res.Auth;
                Sub.Endpoint = res.Endpoint;
                Sub.P256DH = res.P256DH;

                _logger.LogDebug("\n\n" + JsonConvert.SerializeObject(res, Formatting.Indented));

                var user = _repository.GetApplicationUser(Sub.User.ProviderName, Sub.User.ProviderKey);

                var d = _repository.RegisterDevice(new Device()
                {
                    AppUserId = user.Id,
                    P256DH = Sub.P256DH,
                    Endpoint = Sub.Endpoint,
                    Auth = Sub.Auth,
                    CreatedBy = user.Id,
                    UpdatedBy = user.Id
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to register device");
            }
        }
    }
}
