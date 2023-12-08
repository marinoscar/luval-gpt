using Luval.GPT.Data;
using Luval.GPT.Data.Entities;
using Luval.WebGPT.Data.ViewModel;

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
        }

        public void RegisterDevice(DeviceSubscription device)
        {
            try
            {
                if (device == null) throw new ArgumentNullException(nameof(device));
                if (device.User == null) throw new ArgumentException(nameof(device.User));

                var user = _repository.GetApplicationUser(device.User.ProviderName, device.User.ProviderKey);

                var d = _repository.RegisterDevice(new Device()
                {
                    AppUserId = user.Id,
                    P256DH = device.P256DH,
                    Endpoint = device.Endpoint,
                    Auth = device.Auth,
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
