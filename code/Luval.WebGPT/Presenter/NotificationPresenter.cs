﻿using Luval.Framework.Core.Cache;
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
    public class NotificationPresenter : PresenterBase
    {
        public NotificationPresenter(ILogger logger, IRepository repository, IHttpContextAccessor context, ICacheProvider<string, AppUser> userCache) : base(logger, repository, context, userCache)
        {
            Sub = new DeviceSubscription()
            {
                ApplicationServerKey = ConfigManager.Get("VAPIKey"),
                User = new WebUser()
            };
        }

        public DeviceSubscription Sub { get; set; }

        public bool ErrorOcurred { get; set; }

        public string? SystemMessage { get; set; }

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
            IsWorking = true;
            SystemMessage = null;
            ErrorOcurred = false;
            try
            {
                if (Sub == null) throw new ArgumentNullException(nameof(Sub));
                if (Sub.User == null) throw new ArgumentException(nameof(Sub.User));

                var res = await Js.InvokeAsync<PushSubscription>("returnVAPIInfo");

                Sub.Auth = res.Auth;
                Sub.Endpoint = res.Endpoint;
                Sub.P256DH = res.P256DH;

                Logger.LogDebug("JAVASCRIPT OBJECT:\n\n" + JsonConvert.SerializeObject(res, Formatting.Indented));

                var user = Repository.GetApplicationUser(Sub.User.ProviderName, Sub.User.ProviderKey);

                var device = new Device()
                {
                    AppUserId = user.Id,
                    P256DH = res.P256DH,
                    Endpoint = res.Endpoint,
                    Auth = res.Auth,
                    CreatedBy = user.Id,
                    UpdatedBy = user.Id
                };

                Logger.LogDebug("DATA OBJECT:\n\n" + JsonConvert.SerializeObject(device, Formatting.Indented));

                var d = Repository.RegisterDevice(device);
                SystemMessage = "Subscription recorded";
            }
            catch (Exception e)
            {
                ErrorOcurred = true;
                Logger.LogError(e, "Unable to register device");
                SystemMessage = e.Message;
            }
            finally
            {
                IsWorking = false;
            }
        }
    }
}
